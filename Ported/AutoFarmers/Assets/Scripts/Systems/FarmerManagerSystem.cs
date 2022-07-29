using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class FarmerManagerSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireForUpdate<Ecsprefabcreator>();

    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var gameSetting = GetSingleton<Ecsprefabcreator>();
        int farmerCost = gameSetting.FarmerCost;
        Entity farmerPrefab = gameSetting.FarmerPrefab;

        var pecb = ecb.AsParallelWriter();

        Entities
            .ForEach((int entityInQueryIndex, ref NumberOfDestroyedRocks destroyedRocks) =>
            {
                if (destroyedRocks.desRocks >= farmerCost)
                { 
                    Debug.Log("Prefab Created");
                    pecb.Instantiate(entityInQueryIndex, farmerPrefab);
                    destroyedRocks.desRocks -= farmerCost;
                }
                
            }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}