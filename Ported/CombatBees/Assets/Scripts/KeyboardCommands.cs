using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardCommands : MonoBehaviour {
	public Text text;
	void Start () {
		
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			text.enabled = !text.enabled;
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
