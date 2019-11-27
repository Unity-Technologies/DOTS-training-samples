using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardInput : MonoBehaviour 
{
	private Text _text;
	private static bool _showText = true;

	public static KeyboardInput Instance { get; private set; }
	public bool IsSimulationSpeedUpdated { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start () 
	{
		this._text = GetComponent<Text>();
		this._text.enabled = _showText;
	}

	private void Update()
	{
		this.IsSimulationSpeedUpdated = false;
		
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 1f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 2f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 3f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 4f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 5f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 6f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 7f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			this.IsSimulationSpeedUpdated = true;
			Time.timeScale = 8f;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			this.IsSimulationSpeedUpdated = true;
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
