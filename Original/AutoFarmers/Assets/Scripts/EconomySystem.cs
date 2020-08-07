using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Collections;


public struct EconomyData:IComponentData
{
    public int moneyForFarmers;
    public int moneyForDrones;
}

public class EconomySystem:SystemBase
{
    static int farmerPrice = 10;
    static int dronePrice = 50;
    static float timeToNextMoneyIncrease = 0.2f;

    static Entity farmerPrefab;
    static Entity dronePrefab;
    EntityQuery economyQuery;
    EntityQuery storesQuery;
    EntityQuery storesPositionQuery;

    protected override void OnCreate()
    {
        economyQuery = GetEntityQuery(ComponentType.ReadWrite<EconomyData>());
        storesQuery = GetEntityQuery(ComponentType.ReadOnly<Store>(), typeof(SoldToStore));
        storesPositionQuery = GetEntityQuery(ComponentType.ReadOnly<Store>(),typeof(Position2D));
    }

    protected override void OnUpdate()
    {
        if(farmerPrefab == Entity.Null)
        {
            farmerPrefab = GetEntityQuery(typeof(FarmerData_Spawner)).GetSingleton<FarmerData_Spawner>().prefab;
            EntityManager.RemoveComponent<Translation>(farmerPrefab);
            EntityManager.RemoveComponent<Rotation>(farmerPrefab);
            EntityManager.RemoveComponent<Scale>(farmerPrefab);
            EntityManager.RemoveComponent<NonUniformScale>(farmerPrefab);

            EntityManager.AddComponentData<Color>(farmerPrefab,new Color { Value = new float4(1,1,0,1) });
            EntityManager.AddComponentData<Position2D>(farmerPrefab,new Position2D { position = new Unity.Mathematics.float2(0,0) });
            EntityManager.AddComponentData<WorkerDataCommon>(farmerPrefab,new WorkerDataCommon());
            EntityManager.AddComponent<Farmer>(farmerPrefab);
            EntityManager.AddComponent<FarmerIdle>(farmerPrefab);
        }
        if(dronePrefab == Entity.Null)
        {
            dronePrefab = GetEntityQuery(typeof(DroneData_Spawner)).GetSingleton<DroneData_Spawner>().prefab;
            EntityManager.RemoveComponent<Translation>(dronePrefab);
            EntityManager.RemoveComponent<Rotation>(dronePrefab);
            EntityManager.RemoveComponent<Scale>(dronePrefab);
            EntityManager.RemoveComponent<NonUniformScale>(dronePrefab);

            EntityManager.AddComponentData<WorkerDataCommon>(dronePrefab,new WorkerDataCommon());
            EntityManager.AddComponentData<Position2D>(dronePrefab,new Position2D { position = new Unity.Mathematics.float2(0,0) });
            EntityManager.AddComponent<Drone>(dronePrefab);
        }

        var worldEconomy = economyQuery.GetSingleton<EconomyData>();

        // DEBUG
        timeToNextMoneyIncrease -= Time.DeltaTime; 
        if(timeToNextMoneyIncrease < 0)
        {
            Entities
            .WithStructuralChanges()
        .WithName("economy_debug")
            .ForEach((Entity entity,ref EconomyData economies) =>
            {
                economies.moneyForFarmers += 1;
                economies.moneyForDrones += 1;
            }).Run();

            timeToNextMoneyIncrease = 1.0f;
        }

        Entities
        .WithStructuralChanges()
        .WithName("economy_spawn_farmers_drones")
        .ForEach((Entity entity,ref Store store, in SoldToStore sold, in Position2D storePosition) =>
        {
            worldEconomy.moneyForDrones += 1;
            worldEconomy.moneyForFarmers += 1;

            int nbOfFarmersToSpawn = worldEconomy.moneyForFarmers / farmerPrice;
            if(nbOfFarmersToSpawn > 0)
            {
                SpawnFarmers(nbOfFarmersToSpawn,storePosition.position);
                worldEconomy.moneyForFarmers -= nbOfFarmersToSpawn * farmerPrice;
            }

            int nbofDronesToSpawn = worldEconomy.moneyForDrones / dronePrice;
            if(nbofDronesToSpawn > 0)
            {
                SpawnDrones(nbofDronesToSpawn,storePosition.position);
                worldEconomy.moneyForDrones -= nbofDronesToSpawn * dronePrice;
            }
        }).Run();

        EntityManager.RemoveComponent<SoldToStore>(storesQuery);

        Entities
        .WithStructuralChanges()
        .WithName("economy_initial_spawning")
        .ForEach((Entity entity,ref EconomyData economy) =>
        {
            int nbOfFarmersToSpawn = economy.moneyForFarmers / farmerPrice;
            if(nbOfFarmersToSpawn > 0)
            {
                SpawnFarmers(nbOfFarmersToSpawn, GetRandomStorePosition());
                economy.moneyForFarmers -= nbOfFarmersToSpawn * farmerPrice;
            }

            int nbofDronesToSpawn = economy.moneyForDrones / dronePrice;
            if(nbofDronesToSpawn > 0)
            {
                SpawnDrones(nbofDronesToSpawn, GetRandomStorePosition());
                economy.moneyForDrones -= nbofDronesToSpawn * dronePrice;
            }
        }).Run();
    }

    float2 GetRandomStorePosition()
    {
        float2 position = new Unity.Mathematics.float2(0,0);
        var storesArray = storesPositionQuery.ToComponentDataArray<Position2D>(Allocator.TempJob);
        if(storesArray.Length > 0)
        {
            position = storesArray[UnityEngine.Random.Range(0,storesArray.Length)].position;
        }
        storesArray.Dispose();
        return position;
    }

    internal void SpawnFarmers(int count, in float2 pos)
    {
        for(int i = 0;i < count;++i)
        {
            var newFarmer = EntityManager.Instantiate(farmerPrefab);
            EntityManager.AddComponentData<WorkerTeleport>(newFarmer,new WorkerTeleport { position = pos });
        }
    }

    internal void SpawnDrones(int count, in float2 pos)
    {
        for(int i = 0 ; i < count ; ++i)
        {
            var newDrone = EntityManager.Instantiate(dronePrefab);
            EntityManager.AddComponentData<WorkerTeleport>(newDrone,new WorkerTeleport { position = pos });
        }
    }
}
