using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine.SceneManagement;

public partial class KeyboardInputSystem : SystemBase
{
	Config config;
	int TimeScaleSpeed = 1;

	protected override void OnStartRunning()
	{
		config = GetSingleton<Config>();
	}

	protected override void OnUpdate()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			TimeScaleSpeed = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			TimeScaleSpeed = 10;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			TimeScaleSpeed = 20;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			TimeScaleSpeed = 30;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			TimeScaleSpeed = 40;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			TimeScaleSpeed = 50;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			TimeScaleSpeed = 60;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			TimeScaleSpeed = 70;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			TimeScaleSpeed = 80;
		}
		
		Entities
			.WithoutBurst()
			.ForEach((ref Config config) =>
			{
				config.MoveSpeed = TimeScaleSpeed;
            })
			.Run();
	}
}
