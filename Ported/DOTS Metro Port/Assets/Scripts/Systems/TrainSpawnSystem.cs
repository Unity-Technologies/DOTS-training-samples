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
		foreach (var trackBuffer in SystemAPI.Query<DynamicBuffer<BezierPoint>>())
		{
			var trackNativeArray = trackBuffer.AsNativeArray();
			SpawnTrain(trackNativeArray, ecb, config, ref state);
		}
	}

	private void SpawnTrain(NativeArray<BezierPoint> track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity train = BezierSpawnUtility.SpawnOnBezier(config.TrainPrefab, track, 0.5f, ecb);
		SpawnTrainCarriages(train, track, ecb, config, ref state);
	}

	private void SpawnTrainCarriages(Entity train, NativeArray<BezierPoint> track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		for (int i = 0; i < config.CarriagesPerTrain; i++)
		{
			SpawnTrainCarriage(train, i, track, ecb, config, ref state);
		}
	}

	private void SpawnTrainCarriage(Entity train, int carriageIndex, NativeArray<BezierPoint> track, EntityCommandBuffer ecb, Config config, ref SystemState state)
	{
		Entity carriage = BezierSpawnUtility.SpawnOnBezier(config.CarriagePrefab, track, 0.5f, ecb);
		ecb.SetComponent(carriage, new Carriage { Train = train, CarriageIndex = carriageIndex});
	}
}
