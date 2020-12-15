using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class FarmerSpawnerSystem : SystemBase
{
    private const int k_PlantsToSpawnfarmer = 10;
    private const int k_PlantsToSpawnDrone = 50;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<FarmerSpawner>();
    }


    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var spawnerEntity = GetSingletonEntity<FarmerSpawner>();
        var spawner = EntityManager.GetComponentData<FarmerSpawner>(spawnerEntity);

        var amountToSpawn = Mathf.FloorToInt(spawner.FarmerCounter / k_PlantsToSpawnfarmer);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            var instance = ecb.Instantiate(spawner.FarmerPrefab);
        }
        spawner.FarmerCounter -= amountToSpawn * k_PlantsToSpawnfarmer;

        amountToSpawn = Mathf.FloorToInt(spawner.DroneCounter / k_PlantsToSpawnDrone);
        for (int i = 0; i < amountToSpawn; ++i)
        {
            var instance = ecb.Instantiate(spawner.DronePrefab);
        }
        spawner.DroneCounter -= amountToSpawn * k_PlantsToSpawnDrone;

        SetSingleton(spawner);
        ecb.Playback(EntityManager);
    }
}