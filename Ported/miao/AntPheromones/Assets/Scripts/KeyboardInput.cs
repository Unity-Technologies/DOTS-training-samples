using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardInput : MonoBehaviour {
	Text text;

	static bool showText=true;

	void Start () {
		text = GetComponent<Text>();
		text.enabled = showText;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
		{
			Time.timeScale = 1f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			Time.timeScale = 2f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
		{
			Time.timeScale = 3f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			Time.timeScale = 4f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
		{
			Time.timeScale = 5f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			Time.timeScale = 6f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
		{
			Time.timeScale = 7f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
		{
			Time.timeScale = 8f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
		{
			Time.timeScale = 9f;
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			showText = !showText;
			text.enabled = showText;
		}
		else if (Input.GetKeyDown(KeyCode.R))
		{
			Time.timeScale = 1f;

			var entityManager = World.Active.EntityManager;
			entityManager.DestroyEntity(entityManager.UniversalQuery);

			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
