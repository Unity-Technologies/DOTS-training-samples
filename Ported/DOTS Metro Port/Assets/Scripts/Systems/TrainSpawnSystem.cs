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
		int i = 0;
		foreach (var trackBuffer in SystemAPI.Query<DynamicBuffer<BezierPoint>>())
		{
			SpawnTrain(i, ecb, config, ref state);
			i++;
		}
	}

	private void SpawnTrain(int trackIndex, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity train = BezierSpawnUtility.SpawnOnBezier(config.TrainPrefab, trackIndex, 0.5f, ecb);
		SpawnTrainCarriages(train, trackIndex, ecb, config, ref state);
	}

	private void SpawnTrainCarriages(Entity train, int trackIndex, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		for (int i = 0; i < config.CarriagesPerTrain; i++)
		{
			SpawnTrainCarriage(train, i, trackIndex, ecb, config, ref state);
		}
	}

	private void SpawnTrainCarriage(Entity train, int carriageIndex, int trackIndex, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity carriage = BezierSpawnUtility.SpawnOnBezier(config.CarriagePrefab, trackIndex, 0.5f, ecb);
		ecb.SetComponent(carriage, new Carriage { Train = train, CarriageIndex = carriageIndex});
	}
}
