using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardInput : MonoBehaviour 
{
	private Text _text;
	private static bool _showText = true;

	private void Start () 
	{
		this._text = GetComponent<Text>();
		this._text.enabled = _showText;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Time.timeScale = 1f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Time.timeScale = 2f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Time.timeScale = 3f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Time.timeScale = 4f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			Time.timeScale = 5f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			Time.timeScale = 6f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			Time.timeScale = 7f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			Time.timeScale = 8f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			Time.timeScale = 9f;
		}
		else if (Input.GetKeyDown(KeyCode.H))
		{
			this._text.enabled = _showText = !_showText;
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
