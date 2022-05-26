using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial struct FarmerDroneSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FarmMoney>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moneyEntity = SystemAPI.GetSingletonEntity<FarmMoney>();
        var money = SystemAPI.GetSingletonRW<FarmMoney>();
        //money.FarmerMoney += 1;
        //money.DroneMoney += 1;
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        int farmersToSpawn = (money.FarmerMoney / gameConfig.CostToSpawnFarmer) - money.SpawnedFarmers;
        int dronesToSpawn = (money.DroneMoney / gameConfig.CostToSpawnDrone) - money.SpawnedDrones;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var posToSpawn = new int2(
                gameConfig.MapSize.x / 2,
                gameConfig.MapSize.y / 2
        );
        if (money.LastDepositLocaiton.x != 0 && money.LastDepositLocaiton.y != 0)
        {
            posToSpawn = money.LastDepositLocaiton;
        }

        for (int i = 0; i < farmersToSpawn; i++)
        {
            var farmer = ecb.Instantiate(gameConfig.FarmerPrefab);
            ecb.SetComponent(farmer, new Translation()
            {
                Value = new float3(posToSpawn.x, 0.5f, posToSpawn.y),
            });
            ecb.AddBuffer<Waypoint>(farmer);
            money.SpawnedFarmers += 1;
        }

        for (int i = 0; i < dronesToSpawn; i++)
        {
            var drone = ecb.Instantiate(gameConfig.DronePrefab);
            ecb.SetComponent(drone, new Translation()
            {
                Value = new float3(posToSpawn.x, 2f, posToSpawn.y),
            });
            //ecb.AddBuffer<Waypoint>(drone);
            money.SpawnedDrones += 1;
            //UnityEngine.Debug.Log("Drones Spawned: "+ money.SpawnedDrones);
        }

        ecb.SetComponent(moneyEntity, money);
    }
}
