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
		if (Input.GetKeyDown(KeyCode.H)) 
		{
			showText = !showText;
			text.enabled = showText;
		}
		
		if (Input.GetKeyDown(KeyCode.R)) 
		{
			Time.timeScale = 1f;
			
			var entityManager = World.Active.EntityManager;
			entityManager.DestroyEntity(entityManager.UniversalQuery);
			
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
