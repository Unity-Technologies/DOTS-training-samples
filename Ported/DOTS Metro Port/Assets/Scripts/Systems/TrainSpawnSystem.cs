using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
[UpdateAfter(typeof(RailSpawnSystem))]
public partial struct TrainSpawnSystem : ISystem
{
	private EntityQuery m_BaseColorQuery;

	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<Config>();
		m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));
	}

	public void OnDestroy(ref SystemState state)
	{
	}

	public void OnUpdate(ref SystemState state)
	{
		Config config = SystemAPI.GetSingleton<Config>();

		var ecb = GetCommandBuffer(ref state);
		SpawnTrains(ecb, config, ref state);

		state.Enabled = false;
	}

	private EntityCommandBuffer GetCommandBuffer(ref SystemState state)
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
		return ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
	}

	private void SpawnTrains(EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		foreach (var track in SystemAPI.Query<TrackAspect>())
		{
			float pathLength = BezierPath.Get_PathLength(track.TrackBuffer.AsNativeArray());
			float distanceBetweenTrains = pathLength / config.TrainCount;
			float distance = 0.0f;	
			for (int i = 0; i < config.TrainCount; i++)
			{
				SpawnTrain(track, ecb, config, distance, ref state);
				distance += distanceBetweenTrains;
			}
		}
	}

	private void SpawnTrain (TrackAspect track, EntityCommandBuffer ecb, Config config, float distance, ref SystemState state)
	{
		Entity train = BezierSpawnUtility.SpawnOnBezier(config.TrainPrefab, distance, track.Entity, ecb);
		SetColor(train, ecb, track.BaseColor.ValueRO);
		SpawnTrainCarriages(train, track, ecb, config, ref state);
	}

	private void SpawnTrainCarriages(Entity train, TrackAspect track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		for (int i = 0; i < config.CarriagesPerTrain; i++)
		{
			SpawnTrainCarriage(train, i, track, ecb, config, ref state);
		}
	}

	private void SpawnTrainCarriage(Entity train, int carriageIndex, TrackAspect track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity carriage = BezierSpawnUtility.SpawnOnBezier(config.CarriagePrefab, 100, track.Entity, ecb);
		SetColor(carriage, ecb, track.BaseColor.ValueRO);
		ecb.SetComponent(carriage, new Carriage { Train = train, CarriageIndex = carriageIndex});
	}

	private void SetColor(Entity entity, EntityCommandBuffer ecb, URPMaterialPropertyBaseColor color)
	{
		var queryMask = m_BaseColorQuery.GetEntityQueryMask();
		ecb.SetComponentForLinkedEntityGroup(entity, queryMask, color);
	}
}
