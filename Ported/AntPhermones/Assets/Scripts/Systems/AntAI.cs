using NUnit.Framework.Internal;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AntAI: ISystem
{
    private NativeArray<Random> rngs;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Colony>();
        state.RequireForUpdate<Ant>();

        rngs = new NativeArray<Random>(JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        for (var i = 0; i < JobsUtility.MaxJobThreadCount; i++)
        {
            rngs[i] = new Random((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();

        // SteeringRandomizer
        var steeringJob = new SteeringRandomizerJob
        {
            rngs = rngs,
            steeringStrength = colony.randomSteerStrength
        };
        var steeringJobHandle = steeringJob.ScheduleParallel(state.Dependency);



        // PheromoneDetection
        var pheromoneDetectionJob = new PheromoneDetectionJob
        {
            mapSize = (int)colony.mapSize,
            steeringStrength = colony.pheromoneSteerStrength,
            distance = colony.pheromoneSteerDistance,
            pheromones = pheromones
        };
        var pheromoneDetectionJobHandle = pheromoneDetectionJob.ScheduleParallel(steeringJobHandle);



        // ObstacleDetection
        var obstacleJob = new ObstacleDetection
        {
            distance = colony.wallSteerDistance,
            obstacleSize = colony.obstacleSize,
            mapSize = colony.mapSize,
            steeringStrength = colony.wallSteerStrength,
            bucketResolution = colony.bucketResolution,
            buckets = colony.buckets,
        };
        var obstacleJobHandle = obstacleJob.ScheduleParallel(pheromoneDetectionJobHandle);



        // ResourceDetection
        
        
        
        // Dynamics
        var dynamicsJob = new DynamicsJob{ mapSize = colony.mapSize };
        var dynamicsJobHandle = dynamicsJob.Schedule(obstacleJobHandle);


        // Drop Pheromones
        var pheromoneDropJob = new PheromoneDropJob
        {
            deltaTime = SystemAPI.Time.fixedDeltaTime,
            mapSize = (int)colony.mapSize,
            antTargetSpeed = colony.antTargetSpeed,
            pheromoneGrowthRate = colony.pheromoneGrowthRate,
            pheromones = pheromones
        };
        var pheromoneDropJobHandle = pheromoneDropJob.Schedule(dynamicsJobHandle);
        pheromoneDropJobHandle.Complete(); // BUG???



        // Decay Pheromones
        var pheromoneDecayJob = new PheromoneDecayJob
        {
            pheromoneDecayRate = colony.pheromoneDecayRate,
            pheromones = pheromones
        };
        state.Dependency = pheromoneDecayJob.Schedule(pheromones.Length, 100, pheromoneDropJobHandle);
    }
}
