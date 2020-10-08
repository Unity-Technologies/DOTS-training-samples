using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges().ForEach((Entity entity, in BoardSpawner spawner) =>
		{
			for (int x = 0; x < spawner.SizeX; ++x)
			{
				for (int z = 0; z < spawner.SizeZ; ++z)
				{
					float offsetY = Random.Range(0, spawner.RandomYOffset);
					
					var instance = EntityManager.Instantiate(spawner.Prefab);
					SetComponent(instance, new RootTranslation { Value = new float3(x, offsetY, z) });
					SetComponent(instance, new CellInfo { X = x, Z = z });
				}
			}

			EntityManager.RemoveComponent<BoardSpawner>(entity);

		}).Run();
	}
}
