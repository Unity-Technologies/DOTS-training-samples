using UnityEngine;
using UnityEngine.UI;

public class HUDShowHideButton : MonoBehaviour
{
	[Header("Scene")]
	public GameObject _OptionsMenu;

	[Header("Children")]
	public Text _Text;

	public void OnPress()
	{
		_OptionsMenu.SetActive(!_OptionsMenu.activeSelf);
		_Text.text = _OptionsMenu.activeSelf ? "Hide" : "Show";
	}
}
