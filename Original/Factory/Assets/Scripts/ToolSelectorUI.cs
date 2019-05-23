using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelectorUI : MonoBehaviour {
	public ToolButton[] toolButtons;
	static ToolSelectorUI instance;

	private void Start() {
		instance = this;
		UpdateButtonStates();
	}

	void UpdateButtonStates() {
		for (int i=0;i<toolButtons.Length;i++) {
			if ((int)FactoryManager.currentTool==i) {
				toolButtons[i].SetSelected(true);
			} else {
				toolButtons[i].SetSelected(false);
			}
		}
	}

	public static void SetTool(ToolType tool) {
		FactoryManager.currentTool = tool;
		instance.UpdateButtonStates();
	}

	public void OnClickSpawnAgents() {
		SetTool(ToolType.SpawnAgents);
	}
	public void OnClickEraseTiles() {
		SetTool(ToolType.Empty);
	}
	public void OnClickAddWall() {
		SetTool(ToolType.Wall);
	}
	public void OnClickAddResource() {
		SetTool(ToolType.Resource);
	}
	public void OnClickAddCrafter() {
		SetTool(ToolType.Crafter);
	}
	public void OnClickResourcePath() {
		SetTool(ToolType.ResourcePath);
	}
	public void OnClickCrafterPath() {
		SetTool(ToolType.CrafterPath);
	}
}
