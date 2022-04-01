
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AdhocResourceSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var prefabSet = GetSingleton<PrefabSet>();

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			//var instance = EntityManager.Instantiate(prefabSet.ResourcePrefab);
			//float3 pos = new float3(0f, 0f, 0f);
			//var translation = new Translation { Value = pos };
			//EntityManager.SetComponentData(instance, translation);

			var instance = EntityManager.CreateEntity();
			float3 pos = new float3(-45f, 0f, 0f);
			BeeSpawnerComponent beeComponentData = new BeeSpawnerComponent();
			beeComponentData.BeePrefab = prefabSet.YellowBee;
			beeComponentData.BeeCount = 5;
			beeComponentData.BeeSpawnPosition = pos;
			beeComponentData.Process = 1;  // Set this to 1 otherwise it will be ignored. This acts like a toggle for the spawner to process new bees - it then resets it to 0 and stops.
			EntityManager.AddComponentData(instance, beeComponentData);
		}
	}
}