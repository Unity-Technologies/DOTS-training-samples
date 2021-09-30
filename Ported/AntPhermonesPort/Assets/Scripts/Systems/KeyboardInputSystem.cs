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
			
			var entityArray = EntityManager.GetAllEntities();
			foreach (var e in entityArray)
				EntityManager.DestroyEntity(e);
			entityArray.Dispose();

			SceneManager.LoadScene(0);
			TimeScaleSpeed = 1;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			TimeScaleSpeed = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			TimeScaleSpeed = 2;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			TimeScaleSpeed = 3;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			TimeScaleSpeed = 4;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			TimeScaleSpeed = 5;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			TimeScaleSpeed = 6;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			TimeScaleSpeed = 7;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			TimeScaleSpeed = 8;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			TimeScaleSpeed = 9;
		}

		Entities
			.WithoutBurst()
			.ForEach((ref Config config) =>
			{
				config.Speed = TimeScaleSpeed;
			})
			.Run();
	}
}
