using System;
using Unity.Entities;

public class WaterDropApplySystem : SystemBase
{
	EntityQuery m_ForEachQuery;
	
	protected override void OnCreate() {
		RequireForUpdate(m_ForEachQuery);
	}
	
	protected override void OnUpdate()
	{
		var heatMapEntity = GetSingletonEntity<HeatMap>();
		var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
		var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity);

		Entities.WithStoreEntityQueryInField(ref m_ForEachQuery).WithStructuralChanges().ForEach((Entity entity, ref WaterDrop drop) =>
		{
			for (int offsetX = -drop.Range; offsetX <= drop.Range; offsetX++)
			{
				for (int offsetZ = -drop.Range; offsetZ <= drop.Range; offsetZ++)
				{
					if (BoardHelper.TryGet2DArrayIndex(drop.X + offsetX, drop.Z + offsetZ, heatMap.SizeX, heatMap.SizeZ, out var index))
					{
						float value = heatMapBuffer[index].Value;
						heatMapBuffer[index] = new HeatMapElement() { Value = Math.Max(0, value - drop.Strength) };
					}
				}
			}
			
			EntityManager.DestroyEntity(entity);
		}).Run();
	}
}
