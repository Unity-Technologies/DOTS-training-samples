using System;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsManagementSystem : ISystem
{
    private uint RandomSeed;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        RandomSeed = 1;
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<Pheromone>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>().Reinterpret<short>();
        if (pheromones.Length <= 0)
        {
            return;
        }

        var random = Unity.Mathematics.Random.CreateFromIndex(RandomSeed++); //this should be centralised

        var config = SystemAPI.GetSingleton<Config>();

        float2 foodPosition = float2.zero, homePosition = float2.zero;

        foreach (var antTarget in SystemAPI.Query<RefRO<AntsTarget>>())
        {
            if (!antTarget.ValueRO.isHome)
                foodPosition = antTarget.ValueRO.position;
            else
                homePosition = antTarget.ValueRO.position;
        }
        

        var obstacleQuery = SystemAPI.QueryBuilder().WithAll<Obstacle>().Build();
        var obstacles = obstacleQuery.ToComponentDataArray<Obstacle>(state.WorldUpdateAllocator);

        ObstacleAvoidanceJob obstacleAvoidanceJob = new ObstacleAvoidanceJob()
            { config = config, obstacles = obstacles };

        state.Dependency = obstacleAvoidanceJob.ScheduleParallel(state.Dependency);

        RandomSteeringJob randomSteeringJob = new RandomSteeringJob() { config = config, random = random };
        state.Dependency = randomSteeringJob.ScheduleParallel(state.Dependency);

        PheromoneSteeringJob pheromoneSteeringJob = new PheromoneSteeringJob() { config = config, pheromones = pheromones };
        state.Dependency = pheromoneSteeringJob.ScheduleParallel(state.Dependency);

        LineOfSightJob lineOfSightJob = new LineOfSightJob() { config = config, obstacles = obstacles, homePosition = homePosition, foodPosition = foodPosition };
        state.Dependency = lineOfSightJob.ScheduleParallel(state.Dependency);

        GoalSteeringJob goalSteeringJob = new GoalSteeringJob() { config = config, homePosition = homePosition, foodPosition = foodPosition };
        state.Dependency = goalSteeringJob.ScheduleParallel(state.Dependency);
        
        AntsMovementJob antsMovementJob = new AntsMovementJob() { config = config, deltaTime = SystemAPI.Time.DeltaTime };
        state.Dependency = antsMovementJob.ScheduleParallel(state.Dependency);
        
        TargetCollisionJob targetCollisionJob = new TargetCollisionJob() { config = config, homePosition = homePosition, foodPosition = foodPosition };
        state.Dependency = targetCollisionJob.ScheduleParallel(state.Dependency);

        ObstacleCollisionResponseJob obstacleCollisionResponseJob = new ObstacleCollisionResponseJob()
            { config = config, obstacles = obstacles };
        state.Dependency = obstacleCollisionResponseJob.ScheduleParallel(state.Dependency);
        
        PostCollisionUpdateJob postCollisionUpdateJob = new PostCollisionUpdateJob();
        state.Dependency = postCollisionUpdateJob.ScheduleParallel(state.Dependency);

        UpdateTransformJob updateTransformJob = new UpdateTransformJob() { config = config };
        state.Dependency = updateTransformJob.ScheduleParallel(state.Dependency);

        state.Dependency.Complete();

        #region pheromone drop

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
            float excitement = .3f;
            if (ant.ValueRO.hasFood)
            {
                excitement = 1f;
            }

            //excitement *= ant.ValueRO.speed / antSpeed;
            DropPheromones(ant.ValueRO.position, excitement, config.MapSize, config.PheromoneAddSpeed, pheromones,
                SystemAPI.Time.DeltaTime);
        }

        #endregion
    }

    void DropPheromones(Vector2 position, float strength, int mapSize, float trailAddSpeed,
        DynamicBuffer<short> pheromones, float deltaTime)
    {
        int x = (int)math.floor(position.x);
        int y = (int)math.floor(position.y);
        if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
        {
            return;
        }

        int index = ((mapSize - 1) - x) + ((mapSize - 1) - y) * mapSize;
        pheromones[index] +=
            (short)math.floor(trailAddSpeed * short.MaxValue * strength * deltaTime * ((short.MaxValue - pheromones[index]) / (float)short.MaxValue));
        if (pheromones[index] > short.MaxValue - 1)
        {
            pheromones[index] = short.MaxValue;
        }
    }
}
