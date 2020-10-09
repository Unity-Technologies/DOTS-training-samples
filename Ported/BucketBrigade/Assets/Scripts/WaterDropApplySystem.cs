using System;
using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(HeatMapSpreadSystem))]
public class WaterDropApplySystem : SystemBase
{
	EntityQuery m_ForEachQuery;

	private EntityCommandBufferSystem m_ecb;
	
	protected override void OnCreate() {
		RequireForUpdate(m_ForEachQuery);
		m_ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}
	
	protected override void OnUpdate()
	{
		var ecb = m_ecb.CreateCommandBuffer().AsParallelWriter();

		var heatMapEntity = GetSingletonEntity<HeatMap>();
		var heatMap = EntityManager.GetComponentData<HeatMap>(heatMapEntity);
		var heatMapBuffer = EntityManager.GetBuffer<HeatMapElement>(heatMapEntity);

		var handle = Entities.WithStoreEntityQueryInField(ref m_ForEachQuery).ForEach((Entity entity, int entityInQueryIndex, ref WaterDrop drop) =>
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
			
			ecb.DestroyEntity(entityInQueryIndex, entity);
			
		}).Schedule(Dependency);
		
		m_ecb.AddJobHandleForProducer(handle);
		
		//Dependency.Complete();
	}
}
