using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsGenerationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Config>();
        var random = Unity.Mathematics.Random.CreateFromIndex(1);

        for (int i = 0; i < config.AntsPopulation; i++)
        {
            float3 antPosition = new float3(random.NextFloat(-5f, 5f) + config.MapSize * 0.5f, 0f, random.NextFloat(-5f, 5f) + config.MapSize * 0.5f);

            Entity ant = state.EntityManager.Instantiate(config.AntPrefab);
            state.EntityManager.SetComponentData(ant,
                LocalTransform.FromPositionRotationScale(
                    antPosition,
                    quaternion.identity,
                    config.AntRadius));
            state.EntityManager.SetComponentData(ant,
                new Ant
                {
                    position = new float2(antPosition.x, antPosition.z),
                    facingAngle = random.NextFloat(0f, math.PI * 2f),
                    speed = config.AntSpeed,
                    hasFood = false,
                    hasSpottedTarget = false
                });
            state.EntityManager.AddComponentData(ant, 
                new URPMaterialPropertyBaseColor { Value = config.AntHasNoFoodColor });

        }
    }
}
