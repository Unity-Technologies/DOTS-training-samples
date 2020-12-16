using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireCellSpawnerSystem : SystemBase
{
	EntityCommandBufferSystem m_EntityCommandBufferSystem;

	protected override void OnCreate()
	{
		EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}
	
	protected override void OnUpdate()
	{
		EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();
		
		Entities.ForEach((Entity entity, in FireCellSpawner fireCellSpawner) =>
		{
			ecb.DestroyEntity(entity);
			
			for (int x=0; x<FireSimConfig.xDim; ++x)
			{
				for (int y = 0; y < FireSimConfig.yDim; ++y)
				{
					Entity cellEntity = dstManager.Instantiate(fireCellSpawner.Prefab);
					dstManager.AddComponentData(cellEntity, new FireCell
					{
						coord = new int2(x, y)
					});
				}
			}			
			
			Entity fireCell = ecb.Instantiate(fireCellSpawner.Prefab);
			ecb.AddComponent<FireCell>(new FireCell { coord = fireCellSpawner.coord });
		}).Schedule();

		m_EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
	}
}
