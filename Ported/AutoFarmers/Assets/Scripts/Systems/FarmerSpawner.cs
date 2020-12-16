using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class FarmerSpawnerSystem : SystemBase
{
    private const int k_PlantsToSpawnfarmer = 10;
    private const int k_PlantsToSpawnDrone = 50;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CommonData>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var data = GetSingleton<CommonData>();
        var settings = GetSingleton<CommonSettings>();

        var amountToSpawn = Mathf.FloorToInt(data.FarmerCounter / k_PlantsToSpawnfarmer);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            var instance = ecb.Instantiate(settings.FarmerPrefab);
            ecb.AddComponent(instance, new Farmer());
            ecb.AddComponent(instance, new Velocity());
            ecb.AddBuffer<PathNode>(instance);
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
}