/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui.Main.Layouts
{
	/// <summary>
	/// Interface for reset pose layout class.
	/// </summary>
	public interface IResetPoseLayout
	{
		/// <summary>
		/// タイトル
		/// </summary>
		public RectTransformData Title { get; }

		/// <summary>
		/// 説明
		/// </summary>
		public RectTransformData Description { get; }

		/// <summary>
		/// 背景
		/// </summary>
		public RectTransformData Background { get; }

		/// <summary>
		/// コンテンツの表示領域
		/// </summary>
		public LayoutGroupData DisplayArea { get; }

		/// <summary>
		/// リセットポーズ画像
		/// </summary>
		public RectTransformData ResetPoseImage { get; }

		/// <summary>
		/// 次回以降非表示にするトグル
		/// </summary>
		public RectTransformData DoNotShowAgainToggle { get; }

		/// <summary>
		/// リセットポーズ進行中のパネル
		/// </summary>
		public RectTransformData InProgressPanel { get; }

		/// <summary>
		/// リセットポーズ進行中の説明欄
		/// </summary>
		public RectTransformData InProgressDescription { get; }

		/// <summary>
		/// カウントダウン表示テキスト
		/// </summary>
		public RectTransformData CountdownText { get; }

		/// <summary>
		/// リセットポーズ進行中のイメージ
		/// </summary>
		public RectTransformData InProgressImage { get; }

		/// <summary>
		/// リセットポーズ中進捗率を表すスライダー
		/// </summary>
		public RectTransformData ProgressBar { get; }
	}
}