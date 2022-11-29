using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

partial struct AntSpawningSystem : ISystem
{
    private EntityQuery antQuery;

    private Unity.Mathematics.Random random;

    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<Ant>();
        antQuery = state.GetEntityQuery(builder);

        random = new Unity.Mathematics.Random(12345);
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        if (config.TotalAmountOfAnts < antQuery.CalculateEntityCount())
            return;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var randomAngle = 2.0f * math.PI * random.NextFloat();
        var randomDirection = new float2(math.sin(randomAngle), math.cos(randomAngle));

        Entity newAnt = ecb.Instantiate(config.AntPrefab);
        ecb.AddComponent<CurrentDirection>(newAnt, new CurrentDirection { Direction = randomDirection });
    }
}
