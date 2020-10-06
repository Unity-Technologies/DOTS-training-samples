using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges()
			.ForEach((Entity entity, in BoardSpawner spawner, in LocalToWorld world) =>
		{
			for (int x = 0; x < spawner.SizeX; ++x)
			{
				for (int z = 0; z < spawner.SizeZ; ++z)
				{
					float offsetY = Random.Range(0, spawner.RandomYOffset);
					
					var instance = EntityManager.Instantiate(spawner.Prefab);
					SetComponent(instance, new Translation
					{
						Value = world.Position + new float3(x, offsetY, z)
					});
				}
			}
			
			EntityManager.DestroyEntity(entity);
		}).Run();
	}
}
