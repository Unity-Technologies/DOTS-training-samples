using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class KeyboardInput : MonoBehaviour {
	Text text;

	static bool showText=true;

	void Start () {
		text = GetComponent<Text>();
		text.enabled = showText;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			showText = !showText;
			text.enabled = showText;
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
	void OnDestroy()
	{
		var world = World.DefaultGameObjectInjectionWorld;
		if (world != null)
		{
			var entityManager = world.EntityManager;
			entityManager.DestroyEntity(entityManager.UniversalQuery);
		}

	}
}
