using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            var arr = antQuery.ToEntityArray(Allocator.Temp);
            state.EntityManager.DestroyEntity(arr);
        }
        
        var config = SystemAPI.GetSingleton<Config>();

        int currentAntCount = antQuery.CalculateEntityCount();
        int antToBeCreated = math.min(config.TotalAmountOfAnts - currentAntCount, 500);

        if (antToBeCreated < 1)
            return;

        var ants = state.EntityManager.Instantiate(config.AntPrefab, antToBeCreated, Allocator.Temp);

        foreach (var ant in ants)
        {
            var randomAngle = 2.0f * math.PI * random.NextFloat();
            state.EntityManager.SetComponentData<CurrentDirection>(ant, new CurrentDirection { Angle = randomAngle });
            state.EntityManager.SetComponentData<PreviousDirection>(ant, new PreviousDirection { Angle = randomAngle });
        }
    }
}
