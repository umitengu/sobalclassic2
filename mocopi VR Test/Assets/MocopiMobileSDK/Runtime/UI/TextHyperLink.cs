/*
* Copyright 2022 Sony Corporation
*/
using Mocopi.Mobile.Sdk;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mocopi.Ui.Enums;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mocopi.Ui
{
	/// <summary>
	/// UIのTextにハイパーリンクを挿入するコンポーネント
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class TextHyperLink : ViewBase
	{

		[SerializeField]
		private GameObject buttonPrefab;

		private Text textComponent;

		/// <summary>
		/// 書式指定文字列
		/// </summary>
		public string SourceText;

		/// <summary>
		/// リンク先URL。nullでリンクにしない
		/// </summary>
		public string[] LinkUrls;

		/// <summary>
		/// 挿入する文字列
		/// </summary>
		public string[] LinkTexts;

		private bool _selfUpdate;

		private SynchronizationContext synchronizationContext;

		private List<GameObject> _generatedObjects = new List<GameObject>();

		public override EnumView ViewName
		{
			get => EnumView.None;
		}

		protected override void Awake()
		{
			base.Awake();

			this.synchronizationContext = SynchronizationContext.Current;
			textComponent = GetComponent<Text>();
		}

		/// <summary>
		/// UIリサイズ時の処理
		/// </summary>
		private void OnRectTransformDimensionsChange() => Refresh();

		private void Start() => Refresh();

		public void Refresh()
		{
			if ((this._selfUpdate == false) && (this.synchronizationContext != null))
			{
				this.synchronizationContext.Post(_ =>
				{
					this.ApplyLinkText(SourceText, LinkTexts, LinkUrls);
				}, null);
			}
		}

		public void ApplyLinkText(string sourceText, string[] linkTexts, string[] linkUrls)
		{
			//子要素の削除
			foreach (var generatedObject in _generatedObjects)
			{
				Destroy(generatedObject);
			}
			_generatedObjects.Clear();

			if (textComponent == null) return;

			if (linkTexts == null || linkUrls == null)
			{
				textComponent.text = sourceText;
				return;
			}

			textComponent.text = string.Format(sourceText, linkTexts);
			_selfUpdate = true;
			Canvas.ForceUpdateCanvases();
			_selfUpdate = false;

			//vertsは左上、右上、右下、左下の順に一文字ずつ四隅の座標が入っている
			IList<UIVertex> verts = textComponent?.cachedTextGenerator?.verts;
			if (verts == null || verts.Count == 0) return;

			float pixelsPerUnit = textComponent.pixelsPerUnit;

			var linkStartPositionList = MultipleIndexOf(sourceText, "{");
			var linkEndPositionList = MultipleIndexOf(sourceText, "}");
			var linkIndexList = linkStartPositionList.Select((start, index) => int.Parse(sourceText.Substring(start + 1, linkEndPositionList[index] - start - 1))).ToArray();

			for (int index = 0; index < linkStartPositionList.Count; index++)
			{
				//リンクのない置換のみのときはスキップ
				if (string.IsNullOrEmpty(linkUrls[linkIndexList[index]])) continue;

				int startIndex = linkStartPositionList[index];

				if (index > 0)
				{
					//sourceTextの{0}を置換した後の文字数
					for (int i = 0; i < index; i++)
					{
						startIndex += linkTexts[linkIndexList[i]].Length - (linkEndPositionList[i] - linkStartPositionList[i] + 1);
					}
				}

				//空白は描画されないので除外する
				startIndex -= textComponent.text.Substring(0, startIndex + 1).Count(c => c == ' ' || c == '\t' || c == '\n');

				int vertIndex = startIndex * 4;
				if (vertIndex >= verts.Count) break; //文字がUIからはみ出しているとき
				Vector3 topLeft = verts[vertIndex].position;
				Vector3 bottomRight = verts[vertIndex + 2].position;
				float top = topLeft.y;
				float left = topLeft.x;
				float bottom = bottomRight.y;
				float right = bottomRight.x;
				//1文字ずつレンダリングされている座標を調べる
				for (int textPosition = startIndex; textPosition < startIndex + linkTexts[linkIndexList[index]].Length - linkTexts[linkIndexList[index]].Count(c => c == ' ' || c == '\t' || c == '\n'); textPosition++)
				{
					vertIndex = textPosition * 4;
					if (vertIndex >= verts.Count)
					{
						//文字がUIからはみ出しているときはここまでででボタンを作成して終了
						break;
					}
					Vector3 currentTopLeft = verts[vertIndex].position;

					//もし現在の文字が前より下にある場合改行している（xが同じ位置は1文字目なので除く）
					if (currentTopLeft.y < bottom && currentTopLeft.x != topLeft.x)
					{
						//改行しているのでいったん行末でボタンを作成
						CreateLinkButton(top, left, bottom, right, pixelsPerUnit, linkUrls[linkIndexList[index]]);
						topLeft = currentTopLeft;
						top = topLeft.y;
						left = topLeft.x;
						bottom = bottomRight.y;
					}
					bottomRight = verts[vertIndex + 2].position;
					right = bottomRight.x;
					if (top < currentTopLeft.y) top = currentTopLeft.y;
					if (bottom > bottomRight.y) bottom = bottomRight.y;
				}
				//改行なしで完了したときもボタンを作成
				CreateLinkButton(top, left, bottom, right, pixelsPerUnit, linkUrls[linkIndexList[index]]);

			}

			//リンク個所の文字に色を付ける
			textComponent.text = string.Format(sourceText, linkTexts.Select((t, i) => string.IsNullOrEmpty(linkUrls[i]) ? t : $"<color=#67E3FF>{t}</color>").ToArray());
		}

		/// <summary>
		/// このビューが現在表示されているものかどうか
		/// </summary>
		/// <returns></returns>
		protected override bool IsCurrentView() { return false; }

		/// <summary>
		/// 表示中のダイアログが存在するか
		/// </summary>
		/// <returns>true: 表示中のダイアログが存在する</returns>
		protected override bool ExistsDisplayingDialog() { return false; }

		/// <summary>
		/// 端末の戻るボタン押下時の処理
		/// </summary>
		protected override void OnClickDeviceBackKey() { }

		private void CreateLinkButton(float top, float left, float bottom, float right, float pixelsPerUnit, string linkUrl)
		{
			var topLeft = new Vector3(left, top, 0) / pixelsPerUnit;
			var bottomRight = new Vector3(right, bottom, 0) / pixelsPerUnit;
			var buttonObject = Instantiate(buttonPrefab, transform);
			var textRect = (transform as RectTransform);
			var buttonRect = buttonObject.transform as RectTransform;
			buttonRect.anchorMax = new Vector2(0.5f, textRect.anchorMax.y);
			buttonRect.anchorMin = new Vector3(0.5f, textRect.anchorMin.y);
			buttonRect.anchoredPosition = topLeft - new Vector3(0, textRect.pivot.y * textRect.rect.height);
			buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bottomRight.x - topLeft.x);
			buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, topLeft.y - bottomRight.y);
			var button = buttonObject.GetComponent<Button>();
			button.onClick.AddListener(() => base.OpenURLAsync(linkUrl));

			//Android Fix
			var parentPanel = transform.parent;
			if (parentPanel != null)
			{
				var toggle = parentPanel.GetComponent<Toggle>();
				if (toggle != null)
				{
					parentPanel = parentPanel.parent;
					if (parentPanel != null)
					{
						buttonRect.SetParent(parentPanel.transform);
					}
				}
			}

			_generatedObjects.Add(buttonObject);
		}

		private IList<int> MultipleIndexOf(string source, string findText)
		{
			var indexList = new List<int>();
			int index = -1;

			while (true)
			{
				index = source.IndexOf(findText, index + 1);
				if (index < 0) break;
				indexList.Add(index);
			}

			return indexList;
		}

		/// <summary>
		/// ハイパーリンクテキストを削除
		/// </summary>
		public void DestoryLinkText()
		{
			foreach (var generatedObject in _generatedObjects)
			{
				Destroy(generatedObject);
			}
			_generatedObjects.Clear();
		}
	}
}
