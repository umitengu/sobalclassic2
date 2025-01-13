using Mocopi.Ui;
using Mocopi.Ui.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProHyperLink : UiObjectBase, IPointerDownHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		var text = GetComponent<TextMeshProUGUI>();
		var pos = Input.mousePosition;
		var canvas = text.canvas;
		var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
		var index = TMP_TextUtilities.FindIntersectingLink(text, pos, camera);

		if (index == -1) return;

		var linkInfo = text.textInfo.linkInfo[index];
		var url = linkInfo.GetLinkID();

		base.OpenURLAsync(url);
	}
}
