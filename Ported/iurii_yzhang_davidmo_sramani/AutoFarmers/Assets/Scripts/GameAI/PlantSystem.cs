using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using GameAI;
using Pathfinding;

public class PlantSystem : JobComponentSystem
{
	BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
	Entity defaultPlantEntity;

	protected override void OnCreate()
	{
		// Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
		m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
		var initSys = World.GetOrCreateSystem<RenderingMapInit>();
		defaultPlantEntity = initSys.plantEntityPrefab;
	}
	
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var dt = Time.deltaTime;
		var worldSizeHalf = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSizeHalf;

		var plantGrowSpeed = 1;
		// if the plant is growing on the ground
		var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
		var job = Entities
			.WithAll<TagPlant>()
			.WithNone<TagFullyGrownPlant>()
			.ForEach((Entity e, int entityInQueryIndex, ref HealthComponent health) =>
			{
				health.Value += plantGrowSpeed * dt;
				if (health.Value > 5.0)
				{
					ecb.AddComponent<TagFullyGrownPlant>(entityInQueryIndex, e);
				}
			}).Schedule(inputDeps);

		var jobQuery = GetEntityQuery(new EntityQueryDesc {
			All = new ComponentType[] {typeof(TagPlant)},
			None = new ComponentType[] {typeof(TagFullyGrownPlant)}
		});
		if (jobQuery.CalculateEntityCount() > 0) {
			World.GetOrCreateSystem<PathfindingSystem>().DistFieldDirty();
		}

		// if the plant is being picked 
		// DO nothing
		
		// if the plant is being sold
		var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
		var job2 = Entities
			.WithAll<TagFullyGrownPlant>()
			.WithAll<AITagTaskDeliver>()
			.WithoutBurst()
			.ForEach((Entity e, int entityInQueryIndex, ref HealthComponent health) =>
			{
				Debug.Log("Delivering plant");
				health.Value = 0;
				ecb2.RemoveComponent<TagFullyGrownPlant>(entityInQueryIndex, e);
			}).Schedule(job);

		jobQuery = GetEntityQuery(typeof(TagFullyGrownPlant), typeof(AITagTaskDeliver));
		if (jobQuery.CalculateEntityCount() > 0) {
			World.GetOrCreateSystem<PathfindingSystem>().DistFieldDirty();
		}

		var defaultPlant = defaultPlantEntity;
		var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
		var job3 = Entities
			.WithAll<AISubTaskTagPlantSeed>()
			.WithAll<AITagTaskTill>()
			.WithNone<AISubTaskTagComplete>()
			.ForEach((Entity e, int entityInQueryIndex, ref TilePositionable tile) =>
			{
				var wpos = RenderingUnity.Tile2WorldPosition(tile.Position, worldSizeHalf);
				var newEntity = ecb3.Instantiate(entityInQueryIndex, defaultPlant);
				ecb3.SetComponent(entityInQueryIndex, newEntity, new Translation {Value = wpos});
				ecb3.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, e);
			}).Schedule(inputDeps);
		
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
		m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
		var handle = JobHandle.CombineDependencies(job, job2, job3);
			
		return handle;
	}
}