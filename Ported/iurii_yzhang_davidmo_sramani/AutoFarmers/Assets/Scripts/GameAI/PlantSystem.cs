using System;
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
		
		// if the plant is growing on the ground
		var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
		var job = Entities
			.WithAll<TagPlant>()
			.WithNone<TagFullyGrownPlant>()
			.ForEach((Entity e, int entityInQueryIndex, in PlantGrowSpeed s, ref HealthComponent health) =>
			{
				health.Value += s.Value * dt;
				if (health.Value > 5.0)
				{
					ecb.AddComponent<TagFullyGrownPlant>(entityInQueryIndex, e);
				}
			}).Schedule(inputDeps);
		
		// if the plant is being picked 
		// DO nothing
		
		// if the plant is being sold
		var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
		var job2 = Entities
			.WithAll<TagFullyGrownPlant>()
			.WithAll<AITagTaskDeliver>()
			.ForEach((Entity e, int entityInQueryIndex, in PlantGrowSpeed s, ref HealthComponent health) =>
			{
				health.Value = -1;
				ecb.RemoveComponent<TagFullyGrownPlant>(entityInQueryIndex, e);
			}).Schedule(inputDeps);
		
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
		var handle = JobHandle.CombineDependencies(job, job2);
			
		return handle;
	}
}