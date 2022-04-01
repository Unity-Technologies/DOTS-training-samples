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
				}
			}).Run();

		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}