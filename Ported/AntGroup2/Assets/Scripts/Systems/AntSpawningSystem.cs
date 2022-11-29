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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            var arr = antQuery.ToEntityArray(Allocator.Temp);
            foreach (var ant in arr)
            {
                ecb.DestroyEntity(ant);
            }
        }
        
        var config = SystemAPI.GetSingleton<Config>();

        if (config.TotalAmountOfAnts < antQuery.CalculateEntityCount())
            return;

        var randomAngle = 2.0f * math.PI * random.NextFloat();

        Entity newAnt = ecb.Instantiate(config.AntPrefab);
        ecb.AddComponent<CurrentDirection>(newAnt, new CurrentDirection { Angle = randomAngle });
    }
}
