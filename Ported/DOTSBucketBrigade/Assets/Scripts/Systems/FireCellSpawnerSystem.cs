using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class FireCellSpawnerSystem : SystemBase
{
	EntityCommandBufferSystem m_EntityCommandBufferSystem;

	protected override void OnCreate()
	{
		m_EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}
	
	protected override void OnUpdate()
	{
		EntityCommandBuffer ecb = m_EntityCommandBufferSystem.CreateCommandBuffer();

		int xDim = FireSimConfig.xDim;
		int yDim = FireSimConfig.yDim;
		float4 groundColor = new float4(FireSimConfig.color_ground.r, FireSimConfig.color_ground.g, FireSimConfig.color_ground.b, FireSimConfig.color_ground.a);
		float4 fireLowColor = new float4(FireSimConfig.color_fire_low.r, FireSimConfig.color_fire_low.g, FireSimConfig.color_fire_low.b, FireSimConfig.color_fire_low.a);
		float4 fireHighColor = new float4(FireSimConfig.color_fire_high.r, FireSimConfig.color_fire_high.g, FireSimConfig.color_fire_high.b, FireSimConfig.color_fire_high.a);

		Entities.ForEach((Entity entity, in FireCellSpawner fireCellSpawner) =>
		{
			ecb.DestroyEntity(entity);
			
			for (int x=0; x<xDim; ++x)
			{
				for (int y = 0; y < yDim; ++y)
				{
					Entity cellEntity = ecb.Instantiate(fireCellSpawner.Prefab);
					ecb.AddComponent<FireCell>(cellEntity, new FireCell
					{
						coord = new int2(x, y)
					});

					ecb.AddComponent<URPMaterialPropertyBaseColor>(cellEntity, new URPMaterialPropertyBaseColor { Value = groundColor });
				}
			}
		}).Run();
	}
}
