using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// minimal button class, because the default one was adding a bunch of GC
public class ToolButton : MonoBehaviour, IPointerClickHandler {
	public Color idleColor;
	public Color hoverColor;
	public Color selectedColor;
	public ToolType toolType;

	[System.NonSerialized]
	public bool isSelected;

	Image buttonImage;

	private void Awake() {
		buttonImage = GetComponent<Image>();
	}

	public void OnPointerClick(PointerEventData e) {
		ToolSelectorUI.SetTool(toolType);
	}

	public void SetSelected(bool selected) {
		isSelected = selected;
		if (selected) {
			buttonImage.color = selectedColor;
		} else {
			buttonImage.color = idleColor;
		}
	}
}
