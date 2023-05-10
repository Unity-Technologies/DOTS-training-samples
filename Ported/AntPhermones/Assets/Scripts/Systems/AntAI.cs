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
            randomSteering = colony.randomSteering
        };
        var steeringJobHandle = steeringJob.Schedule(state.Dependency);
        
        

        // ObstacleDetection
        var obstaclesQuery = new EntityQueryBuilder(Allocator.Temp);
        obstaclesQuery.WithAll<LocalTransform>();
        obstaclesQuery.WithNone<Ant>();
        obstaclesQuery.WithNone<Home>();
        obstaclesQuery.WithNone<Resource>();
        var obstacleJob = new ObstacleDetection
        {
            distance = 1.25f,
            obstacleSize = colony.obstacleSize,
            steeringStrength = colony.wallSteerStrength,
            obstacles = obstaclesQuery.Build(ref state).ToComponentDataArray<LocalTransform>(Allocator.TempJob)
        };
        var obstacleJobHandle = obstacleJob.Schedule(steeringJobHandle);
        
        
        
        // PheromoneDetection
        
        
        
        // ResourceDetection
        
        
        
        // Dynamics
        var dynamicsJob = new DynamicsJob{ mapSize = colony.mapSize };
        var dynamicsJobHandle = dynamicsJob.Schedule(obstacleJobHandle); // TODO: combine this handle with pheromone and resource detection handles



        // Drop Pheromones
        var pheromoneDropJob = new PheromoneDropJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            mapSize = (int)colony.mapSize,
            antTargetSpeed = colony.antTargetSpeed,
            pheromoneGrowthRate = colony.pheromoneGrowthRate,
            pheromones = pheromones
        };
        var pheromoneDropJobHandle = pheromoneDropJob.Schedule(dynamicsJobHandle);



        // Decay Pheromones
        var pheromoneDecayJob = new PheromoneDecayJob
        {
            pheromoneDecayRate = colony.pheromoneDecayRate,
            pheromones = pheromones
        };
        state.Dependency = pheromoneDecayJob.Schedule(pheromoneDropJobHandle);
    }
}
