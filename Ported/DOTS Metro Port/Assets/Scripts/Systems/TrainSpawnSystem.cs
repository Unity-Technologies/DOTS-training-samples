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
			SpawnTrain(trackNativeArray, ecb, config);
		}
			
	}

	private void SpawnTrain(NativeArray<BezierPoint> track, EntityCommandBuffer ecb, Config config)
	{
		for (int i = 1; i < 50; i++)
		{
			BezierSpawnUtility.SpawnOnBezier(config.CarriagePrefab, track, 1.0f/(float)i, ecb);
		}
	}
}
