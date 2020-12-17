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

        var amountToSpawn = Mathf.FloorToInt(data.FarmerCounter / k_PlantsToSpawnfarmer);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            AddFarmer(ecb, settings.FarmerPrefab, new int3(0, 0, 0));
        }
        data.FarmerCounter -= amountToSpawn * k_PlantsToSpawnfarmer;

        amountToSpawn = Mathf.FloorToInt(data.DroneCounter / k_PlantsToSpawnDrone);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            var instance = ecb.Instantiate(settings.DronePrefab);
            ecb.AddComponent(instance, new Drone());
            ecb.AddComponent(instance, new Velocity());
        }
        data.DroneCounter -= amountToSpawn * k_PlantsToSpawnDrone;

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