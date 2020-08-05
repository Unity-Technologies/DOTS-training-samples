using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;


public struct EconomyData:IComponentData
{
    public int moneyForFarmers;
    public int moneyForDrones;
}


public class EconomySystem:SystemBase
{
    static int farmerPrice = 10;

    protected override void OnUpdate()
    {
        Entities
        .WithStructuralChanges()
        .ForEach((Entity entity,ref EconomyData economies, in FarmerData farmerData) =>
        {
            int nbOfFarmersToSpawn = economies.moneyForFarmers / farmerPrice;
            if(nbOfFarmersToSpawn > 0)
            {
                SpawnFarmers(nbOfFarmersToSpawn,farmerData);
                economies.moneyForFarmers -= nbOfFarmersToSpawn * farmerPrice;
            }
        }).Run();
    }

    internal void SpawnFarmers(int count, in FarmerData farmerData)
    {
        for(int i = 0; i < count;++i)
        {
            var newFarmer = EntityManager.Instantiate(farmerData.farmerEntity);
            EntityManager.RemoveComponent<Translation>(newFarmer);
            EntityManager.RemoveComponent<Rotation>(newFarmer);
            EntityManager.RemoveComponent<Scale>(newFarmer);
            EntityManager.RemoveComponent<NonUniformScale>(newFarmer);
            EntityManager.AddComponent<FarmerIdle>(newFarmer);
            EntityManager.AddComponentData<Position2D>(newFarmer,new Position2D { position = new float2(0,0) });
            EntityManager.AddComponentData<Color>(newFarmer, new Color {  Value = new float4(1,1,0,1) });
            int posX = UnityEngine.Random.Range(0,10);
            int posY = UnityEngine.Random.Range(0,10);
            EntityManager.AddComponentData<WorkerTeleport>(newFarmer,new WorkerTeleport { position = new float2(posX,posY) });
            EntityManager.RemoveComponent<FarmerData>(newFarmer);
        }
    }
}
