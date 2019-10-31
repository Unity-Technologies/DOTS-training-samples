using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using GameAI;

public class PlantSystem : JobComponentSystem
{
	BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

	protected override void OnCreate()
	{
		// Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
		m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
	}
	
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var dt = Time.deltaTime;
		var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

		var job = Entities
			.WithAll<TagPlant>()
			.WithNone<TagFullyGrownPlant>()
			.ForEach((Entity e, int entityInQueryIndex, ref PlantGrowSpeed s, ref HealthComponent health) =>
		{
			health.Value += s.Value * dt;
			if (health.Value > 5.0)
			{
				ecb.AddComponent<TagFullyGrownPlant>(entityInQueryIndex, e);
			}
		}).Schedule(inputDeps);

		return job;
	}
}