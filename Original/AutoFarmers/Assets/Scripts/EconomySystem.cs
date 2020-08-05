using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;


public struct EconomyData:IComponentData
{
    public int moneyForFarmers;
    public int moneyForDrones;
}

public class EconomySystem:SystemBase
{
    static int farmerPrice = 10;
    static int dronePrice = 50;
    static float timeToNextMoneyIncrease = 1.0f;


    protected override void OnUpdate()
    {
        // DEBUG
        timeToNextMoneyIncrease -= Time.DeltaTime; 
        if(timeToNextMoneyIncrease < 0)
        {
            Entities
            .WithStructuralChanges()
            .ForEach((Entity entity,ref EconomyData economies) =>
            {
                economies.moneyForFarmers += 1;
                economies.moneyForDrones += 1;
            }).Run();

            timeToNextMoneyIncrease = 1.0f;
        }

        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity,ref EconomyData economies,in FarmerData_Spawner farmerData) =>
        {
            int nbOfFarmersToSpawn = economies.moneyForFarmers / farmerPrice;
            if(nbOfFarmersToSpawn > 0)
            {
                SpawnFarmers(nbOfFarmersToSpawn,farmerData);
                economies.moneyForFarmers -= nbOfFarmersToSpawn * farmerPrice;
            }
        }).Run();

        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity,ref EconomyData economies,in DroneData_Spawner droneData) =>
        {
            int nbofDronesToSpawn = economies.moneyForDrones / dronePrice;
            if(nbofDronesToSpawn > 0)
            {
                SpawnDrones(nbofDronesToSpawn, droneData);
                economies.moneyForDrones -= nbofDronesToSpawn * dronePrice;
            }
        }).Run();
    }

    internal void SpawnFarmers(int count,in FarmerData_Spawner farmerData)
    {
        for(int i = 0;i < count;++i)
        {
            var newFarmer = EntityManager.Instantiate(farmerData.prefab);
            EntityManager.RemoveComponent<Translation>      (newFarmer);
            EntityManager.RemoveComponent<Rotation>         (newFarmer);
            EntityManager.RemoveComponent<Scale>            (newFarmer);
            EntityManager.RemoveComponent<NonUniformScale>  (newFarmer);

            EntityManager.AddComponent<Farmer>(newFarmer);
            EntityManager.AddComponent<FarmerIdle>(newFarmer);
            EntityManager.AddComponentData<Position2D>(newFarmer,new Position2D { position = new Unity.Mathematics.float2(0,0) });
            int posX = UnityEngine.Random.Range(0,10);
            int posY = UnityEngine.Random.Range(0,10);
            EntityManager.AddComponentData<WorkerTeleport>(newFarmer,new WorkerTeleport { position = new Unity.Mathematics.float2(posX,posY) });
            EntityManager.AddComponentData<WorkerDataCommon>(newFarmer,new WorkerDataCommon());
        }
    }

    internal void SpawnDrones(int count,in DroneData_Spawner droneData)
    {
        for(int i = 0;i < count;++i)
        {/*
            var newDrone = EntityManager.Instantiate(droneData.prefab);

            EntityManager.RemoveComponent<Translation>    (newDrone);
            EntityManager.RemoveComponent<Rotation>       (newDrone);
            EntityManager.RemoveComponent<Scale>          (newDrone);
            EntityManager.RemoveComponent<NonUniformScale>(newDrone);

            EntityManager.AddComponent<Drone>(newDrone);
            //TODO: make it 3D
            EntityManager.AddComponentData<Position2D>(newDrone,new Position2D { position = new Unity.Mathematics.float2(0,0) });
            int posX = UnityEngine.Random.Range(0,10);
            int posY = UnityEngine.Random.Range(0,10);
            EntityManager.AddComponentData<WorkerTeleport>(newDrone,new WorkerTeleport { position = new Unity.Mathematics.float2(posX,posY) });*/
        }
    }
}
