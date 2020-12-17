using Unity.Burst.Intrinsics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FarmInitializeSystem))]
public class SpawnerSystem : SystemBase
{
    private const int k_PlantsToSpawnfarmer = 10;
    private const int k_PlantsToSpawnDrone = 50;

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var data = GetSingleton<CommonData>();
        var settings = GetSingleton<CommonSettings>();

        var amountToSpawn = (int)math.floor(data.FarmerCounter / k_PlantsToSpawnfarmer);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            AddFarmer(ecb, settings.FarmerPrefab, new int3(0, 0, 0));
        }
        data.FarmerCounter -= amountToSpawn * k_PlantsToSpawnfarmer;

        var random = new Unity.Mathematics.Random(1234);
        if (data.MoneyForDrones >= 50)
        {
            for (int i = 0; i < 5; i++)
            {
                int x = 10;
                int y = 10;
                var position = new float3(x + .5f, 0f, y + .5f);
                var instance = ecb.Instantiate(settings.DronePrefab);
                ecb.SetComponent(instance, new Translation { Value = position });
                ecb.AddComponent(instance, new Drone
                {
                    smoothPosition = position,
                    moveSmooth = data.MoveSmoothForDrones
                });

                ecb.AddComponent(instance, new DroneCheckPoints
                {
                    hoverHeight = random.NextFloat(2, 3),
                    storePosition = new int2(x, y),
                });

                ecb.AddComponent(instance, new Velocity());
                ecb.AddBuffer<PathNode>(instance);
            }

            data.MoneyForDrones -= 50;
        }
        SetSingleton(data);
        
        ecb.Playback(EntityManager);
    }

    private static Entity Add(EntityCommandBuffer ecb, Entity prefab, int3 position)
    {
        var instance = ecb.Instantiate(prefab);
        
        var translation = new Translation { Value = new float3(position) };
        ecb.SetComponent(instance, translation);

        return instance;
    }

    public static Entity AddFarmer(EntityCommandBuffer ecb, Entity prefab, int3 position)
    {
        var instance = Add(ecb, prefab, position);
        ecb.AddComponent(instance, new Farmer());
        ecb.AddComponent(instance, new Velocity());
        ecb.AddBuffer<PathNode>(instance);
        return instance;
    }
}