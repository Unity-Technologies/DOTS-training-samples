using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

internal struct CellInfo
{
	public int x;
	public int z;
}

public class BoardSpawnerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges()
			.ForEach((Entity entity, in BoardSpawner spawner) =>
			{
				NativeArray<CellInfo> cellsOnFire = new NativeArray<CellInfo>(spawner.InitialOnFireCellCount, Allocator.Temp, NativeArrayOptions.ClearMemory);

				for (int i = 0; i < spawner.InitialOnFireCellCount; ++i)
				{
					int cellX = Random.Range(0, spawner.SizeX);
					int cellZ = Random.Range(0, spawner.SizeZ);

					cellsOnFire[i] = new CellInfo { x = cellX, z = cellZ };
				}

				for (int x = 0; x < spawner.SizeX; ++x)
				{
					for (int z = 0; z < spawner.SizeZ; ++z)
					{
						float offsetY = Random.Range(0, spawner.RandomYOffset);

						var instance = EntityManager.Instantiate(spawner.Prefab);
						SetComponent(instance, new RootTranslation { Value = new float3(x, offsetY, z) });

						float cellValue = spawner.InitialIntensity;
						for (int j = 0; j < cellsOnFire.Length; ++j)
						{
							if (cellsOnFire[j].x == x && cellsOnFire[j].z == z)
							{
								cellValue = Random.Range(spawner.InitialOnFireIntensityMin, spawner.InitialOnFireIntensityMax);
								break;
							}
						}

						SetComponent(instance, new Intensity { Value = cellValue } );
					}
				}

				EntityManager.DestroyEntity(entity);
		}).Run();
	}
}
