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
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			showText = !showText;
			text.enabled = showText;
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			Time.timeScale = 1f;
			var em = World.DefaultGameObjectInjectionWorld.EntityManager;
			em.DestroyEntity(em.UniversalQuery);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
