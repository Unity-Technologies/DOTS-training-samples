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

        /*{
            //init
            float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
            
            float2 position = ant.ValueRO.position;

            var facingAngle = ant.ValueRO.facingAngle;

            float2 directionToTarget;
            float distanceToTarget;

            var isTargetVisible = true;
            float dx, dy, dist;

            float speed, vx, vy, ovx, ovy;
        }*/

        var obstacleQuerry = SystemAPI.QueryBuilder().WithAll<Obstacle>().Build();
        var obstacles = obstacleQuerry.ToComponentDataArray<Obstacle>(state.WorldUpdateAllocator);

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

        state.Dependency.Complete();
        float speed, vx, vy, ovx, ovy;

        foreach (var (ant, transform, color) in SystemAPI
                     .Query<RefRW<Ant>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>())
        {

            float2 position = ant.ValueRO.position;

            var facingAngle = ant.ValueRO.facingAngle;

            #region movement

            {
                speed = ant.ValueRO.speed * SystemAPI.Time.DeltaTime;

                vx = math.cos(facingAngle) * speed;
                vy = math.sin(facingAngle) * speed;
                ovx = vx;
                ovy = vy;

                if (position.x + vx < 0f || position.x + vx > config.MapSize)
                {
                    vx = -vx;
                }

                position.x += vx;

                if (position.y + vy < 0f || position.y + vy > config.MapSize)
                {
                    vy = -vy;
                }

                position.y += vy;
            }

            #endregion
        }

        foreach (var (ant, color) in SystemAPI
                     .Query<RefRW<Ant>, RefRW<URPMaterialPropertyBaseColor>>())
        {
            #region target collision reponse

            {
                float2 targetPosition = ant.ValueRO.hasFood ? homePosition : foodPosition;
                var directionToTarget = targetPosition - ant.ValueRO.position;
                var distanceToTarget = math.lengthsq(directionToTarget);
                if (distanceToTarget < config.TargetRadius * config.TargetRadius)
                {
                    ant.ValueRW.hasFood = !ant.ValueRO.hasFood;
                    color.ValueRW.Value = ant.ValueRO.hasFood ? config.AntHasFoodColor : config.AntHasNoFoodColor;
                    ant.ValueRW.facingAngle += math.PI;
                }
            }

            #endregion
        }

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {

            #region obstacle collision response

            {
                float dx, dy, dist;
                
                foreach (var obstacle in SystemAPI.Query<RefRO<Obstacle>>())
                {
                    dx = ant.ValueRO.position.x - obstacle.ValueRO.position.x;
                    dy = ant.ValueRO.position.y - obstacle.ValueRO.position.y;
                    float sqrDist = dx * dx + dy * dy;
                    if (sqrDist < config.ObstacleRadius * config.ObstacleRadius)
                    {
                        dist = math.sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;
                        ant.ValueRW.position.x = obstacle.ValueRO.position.x + dx * config.ObstacleRadius;
                        ant.ValueRW.position.y = obstacle.ValueRO.position.y + dy * config.ObstacleRadius;

                        vx -= dx * (dx * vx + dy * vy) * 1.5f;
                        vy -= dy * (dx * vx + dy * vy) * 1.5f;
                    }
                }
            }

            #endregion
        }

        foreach (var (ant, transform, color) in SystemAPI
                     .Query<RefRW<Ant>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>())
        {

            #region post collision

            {
                if (ovx != vx || ovy != vy)
                {
                    ant.ValueRW.facingAngle = math.atan2(vy, vx);
                }
            }

            #endregion

        }

        foreach (var ant in SystemAPI.Query<RefRW<Ant>>())
        {
            #region pheromone drop

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

        foreach (var (ant, transform) in SystemAPI
                     .Query<RefRO<Ant>, RefRW<LocalTransform>>())
        {
            #region update transform

            {
                transform.ValueRW = LocalTransform.FromPositionRotationScale(
                    new float3(ant.ValueRO.position.x, 0f, ant.ValueRO.position.y),
                    quaternion.AxisAngle(new float3(0, 1, 0), -ant.ValueRO.facingAngle),
                    config.AntRadius * 2);
            }

            #endregion
        }
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
