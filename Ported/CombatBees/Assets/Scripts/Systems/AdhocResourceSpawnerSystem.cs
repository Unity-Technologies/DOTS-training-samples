using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;


public partial class AdhocResourceSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		var prefabSet = GetSingleton<PrefabSet>();
		Entities
			.ForEach((Entity entity, ref AdhocResourceSpawnerComponent spawner) =>
			{
				// Destroying the current entity is a classic ECS pattern,
				// when something should only be processed once then forgotten.
				if (spawner.Process == 1)
				{
					spawner.Process = 0;
					ecb.DestroyEntity(entity);

					var instance = ecb.Instantiate(spawner.ResourcePrefab);
					float3 pos = spawner.ResourceSpawnPosition;
					var translation = new Translation { Value = pos };
					ecb.SetComponent(instance, translation);

					//for (int i = 0; i < spawner.BeeCount; ++i)
					//{
					//	var instance = ecb.Instantiate(spawner.BeePrefab);
					//	var translation = new Translation { Value = spawner.BeeSpawnPosition };
					//	ecb.SetComponent(instance, translation);
					//	ecb.SetComponent(instance, new VelocityComponent() { Value = random.NextFloat3Direction() * MAX_SPAWN_SPEED });
					//	ecb.SetComponent(instance, new BeeBaseSizeComponent() { Value = random.NextFloat(MIN_BEE_SIZE, MAX_BEE_SIZE) });
					//}
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();



		/*

				if (Input.GetKeyUp(KeyCode.Mouse0))
				{
					var instance = EntityManager.Instantiate(prefabSet.ResourcePrefab);
					float3 pos = new float3(0f, 0f, 0f);
					var translation = new Translation { Value = pos };
					EntityManager.SetComponentData(instance, translation);

					//var instance = EntityManager.CreateEntity();
					//float3 pos = new float3(-45f, 0f, 0f);
					//BeeSpawnerComponent beeComponentData = new BeeSpawnerComponent();
					//beeComponentData.BeePrefab = prefabSet.YellowBee;
					//beeComponentData.BeeCount = 5;
					//beeComponentData.BeeSpawnPosition = pos;
					//beeComponentData.Process = 1;  // Set this to 1 otherwise it will be ignored. This acts like a toggle for the spawner to process new bees - it then resets it to 0 and stops.
					//EntityManager.AddComponentData(instance, beeComponentData);
				}
		*/
	}
}