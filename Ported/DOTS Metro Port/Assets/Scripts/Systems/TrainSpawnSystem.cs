using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(RailSpawnSystem))]
public partial struct TrainSpawnSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
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
			SpawnTrain(track, ecb, config, ref state);
		}
	}

	private void SpawnTrain (TrackAspect track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity train = BezierSpawnUtility.SpawnOnBezier(config.TrainPrefab, 100, track.Entity, ecb);
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
		ecb.SetComponent(carriage, new Carriage { Train = train, CarriageIndex = carriageIndex});
	}
}
