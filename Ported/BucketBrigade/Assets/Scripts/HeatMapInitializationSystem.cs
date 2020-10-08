using System;
using Unity.Collections;
using Unity.Entities;
using Random = UnityEngine.Random;

[UpdateBefore(typeof(CellDisplaySystem))]
[UpdateBefore(typeof(BoardSpawnerSystem))]
[UpdateBefore(typeof(HeatMapSpreadSystem))]
[UpdateBefore(typeof(AgentUpdateSystem))]
[UpdateBefore(typeof(WaterDropApplySystem))]
public class HeatInitializationSystem : SystemBase
{
	protected override void OnStartRunning()
	{
		Entities.WithAll<FireSimulationSettings>()
			.WithoutBurst()
			.ForEach((ref FireSimulationSettings settings) =>
			{
				SetSingleton(settings);
			})
			.Run();
	}
	
	protected override void OnUpdate()
	{
		Entities.WithStructuralChanges().ForEach((Entity entity, ref HeatMap heatMap, in BoardSpawner spawner) =>
		{
			var heatMapBuffer = EntityManager.AddBuffer<HeatMapElement>(entity);
			
			var cellsOnFire = new NativeArray<CellInfo>(spawner.InitialOnFireCellCount, Allocator.Temp);
		
			for (int i = 0; i < spawner.InitialOnFireCellCount; ++i)
			{
				int cellX = Random.Range(0, spawner.SizeX);
				int cellZ = Random.Range(0, spawner.SizeZ);

				cellsOnFire[i] = new CellInfo {X = cellX, Z = cellZ};
			}
		
			for (int x = 0; x < spawner.SizeX; ++x)
			{
				for (int z = 0; z < spawner.SizeZ; ++z)
				{
					float cellValue = spawner.InitialIntensity;
					for (int j = 0; j < cellsOnFire.Length; ++j)
					{
						if (cellsOnFire[j].X == x && cellsOnFire[j].Z == z)
						{
							cellValue = Random.Range(spawner.InitialOnFireIntensityMin,	spawner.InitialOnFireIntensityMax);
							break;
						}
					}
					
					heatMapBuffer.Add(new HeatMapElement { Value = cellValue });
				}
			}

			heatMap.SizeX = spawner.SizeX;
			heatMap.SizeZ = spawner.SizeZ;
			
			SetSingleton(heatMap);

			cellsOnFire.Dispose();

		}).Run();
	}
}
