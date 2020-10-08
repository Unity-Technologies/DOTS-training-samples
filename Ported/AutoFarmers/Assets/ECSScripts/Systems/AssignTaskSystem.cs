using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditorInternal;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AssignTaskSystem : SystemBase
{
    Random m_Random;
    EntityCommandBufferSystem m_ECBSystem;

    EntityQuery cropsQuery;
    EntityQuery forestsQuery;

    protected override void OnCreate()
    {
        m_Random = new Random(666);
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        cropsQuery = GetEntityQuery(typeof(Crop), ComponentType.Exclude<Assigned>());
        forestsQuery = GetEntityQuery(typeof(Forest), ComponentType.Exclude<Assigned>());
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        //NativeArray<Entity> crops = cropsQuery.ToEntityArray(Allocator.TempJob);
        // // Loop over all idle farmers, assigning a pickup crop task
        // Entities.
        //     WithName("assign_pickup_crop_task").
        //     WithAll<Farmer>().
        //     WithNone<DropOffCropTask>().
        //     WithNone<PickUpCropTask>().
        //     WithReadOnly(crops).
        //     WithDisposeOnCompletion(crops).
        //     ForEach(
        //     (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
        //     {
        //         // Find nearest crop
        //         float minDistSq = float.MaxValue;
        //         Entity nearestCropEntity = Entity.Null;
        //         float2 nearestCropPos = float2.zero;
        //         for (int i = 0; i < crops.Length; i++)
        //         {
        //             Translation cropTranslation = GetComponent<Translation>(crops[i]);
        //             float2 cropPos = new float2(cropTranslation.Value.x, cropTranslation.Value.z);
        //             float distSq = math.distancesq(cropPos, farmerPos.Value);
        //             if (minDistSq > distSq)
        //             {
        //                 minDistSq = distSq;
        //                 nearestCropEntity = crops[i];
        //                 nearestCropPos = cropPos;
        //             }
        //         }
        //
        //         if (nearestCropEntity != Entity.Null)
        //         {
        //             ecb.AddComponent<PickUpCropTask>(entityInQueryIndex, farmerEntity);
        //             ecb.AddComponent<TargetEntity>(entityInQueryIndex, farmerEntity);
        //             ecb.SetComponent(entityInQueryIndex, farmerEntity, new TargetEntity { target = nearestCropEntity, targetPosition = nearestCropPos });
        //             ecb.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
        //         }
        //
        //     }).ScheduleParallel();
        
        
        NativeArray<Entity> forests = forestsQuery.ToEntityArray(Allocator.TempJob);
        
        // Loop over all idle farmers, assigning a chop forest task
        Entities.
            WithName("assign_chop_forest_task").
            WithAll<Farmer>().
            WithNone<DropOffCropTask>().
            WithNone<PickUpCropTask>().
            WithNone<ChopForestTask>().
            WithNone<ChoppingTask>().
            WithReadOnly(forests).
            WithDisposeOnCompletion(forests).
            ForEach(
            (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
            {
                // Find nearest crop
                float minDistSq = float.MaxValue;
                Entity nearestForestEntity = Entity.Null;
                float2 nearestForestPos = float2.zero;
                for (int i = 0; i < forests.Length; i++)
                {
                    Translation forestTranslation = GetComponent<Translation>(forests[i]);
                    float2 forestPos = new float2(forestTranslation.Value.x, forestTranslation.Value.z);
                    float distSq = math.distancesq(forestPos, farmerPos.Value);
                    if (minDistSq > distSq)
                    {
                        minDistSq = distSq;
                        nearestForestEntity = forests[i];
                        nearestForestPos = forestPos;
                    }
                }
        
                if (nearestForestEntity != Entity.Null)
                {
                    ecb.AddComponent<ChopForestTask>(entityInQueryIndex, farmerEntity);
                    ecb.AddComponent(entityInQueryIndex, farmerEntity, new TargetEntity { target = nearestForestEntity, targetPosition = nearestForestPos });
                    ecb.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                }
        
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);


    }

}
