using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JumpTheGun {

	public class OptionsToggle : MonoBehaviour {

		[Header("Scene")]
		public GameObject optionsMenu;

		[Header("Children")]
		public Text text;

		public void ButtonPressed(){
			optionsMenu.SetActive(!optionsMenu.activeSelf);
			text.text = optionsMenu.activeSelf ? "Hide" : "Show";
		}

	}

}