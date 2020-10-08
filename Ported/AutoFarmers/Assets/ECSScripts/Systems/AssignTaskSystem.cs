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

    EntityQuery cropPlainsQuery;
    EntityQuery forestsQuery;
    EntityQuery emptyPlainsQuery;

    protected override void OnCreate()
    {
        m_Random = new Random(666);

        cropPlainsQuery = GetEntityQuery(typeof(CropReference), ComponentType.Exclude<Assigned>());
        forestsQuery = GetEntityQuery(typeof(Forest), ComponentType.Exclude<Assigned>());
        emptyPlainsQuery = GetEntityQuery(
            typeof(Plains),
            ComponentType.Exclude<Assigned>(),
            ComponentType.Exclude<Forest>(),
            ComponentType.Exclude<Depot>(),
            ComponentType.Exclude<Tilled>()
        );
    }

    protected override void OnUpdate()
    {
        var gameState = GetSingleton<GameState>();
        var ecb = new EntityCommandBuffer(Allocator.TempJob, PlaybackPolicy.MultiPlayback);
        var ecbWriter = ecb.AsParallelWriter();

        int nextTask = m_Random.NextInt(0, 3);

        if(nextTask == 0)
        {
            NativeArray<Entity> cropPlains = cropPlainsQuery.ToEntityArray(Allocator.TempJob);
            
            // Loop over all idle farmers, assigning a pickup crop task
            Entities.WithName("assign_pickup_crop_task").
                WithAll<Farmer>().
                WithNone<DropOffCropTask>().
                WithNone<PickUpCropTask>().
                WithNone<TillTask>().
                WithNone<ChopForestTask>().
                WithNone<ChoppingTask>().
                WithReadOnly(cropPlains).
                WithDisposeOnCompletion(cropPlains).ForEach(
                    (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
                    {
                        // Find nearest crop
                        float minDistSq = float.MaxValue;
                        Entity nearestTarget = Entity.Null;
                        float2 nearestTargetPos = float2.zero;
                        for(int i = 0; i < cropPlains.Length; i++)
                        {
                            Translation cropTranslation = GetComponent<Translation>(cropPlains[i]);
                            float2 cropPos = new float2(cropTranslation.Value.x, cropTranslation.Value.z);
                            float distSq = math.distancesq(cropPos, farmerPos.Value);
                            if(minDistSq > distSq && distSq < gameState.MaximumTaskDistance)
                            {
                                minDistSq = distSq;
                                nearestTarget = cropPlains[i];
                                nearestTargetPos = cropPos;
                            }
                        }

                        if(nearestTarget != Entity.Null)
                        {
                            ecbWriter.AddComponent<PickUpCropTask>(entityInQueryIndex, farmerEntity);
                            ecbWriter.AddComponent(entityInQueryIndex, farmerEntity,
                                new TargetEntity { target = nearestTarget, targetPosition = nearestTargetPos });
                            ecbWriter.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                        }

                    }).ScheduleParallel();

            // Complete task assignment and structural changes
            Dependency.Complete();
            ecb.Playback(EntityManager);
        }
        
        if(nextTask <= 1)
        {
            NativeArray<Entity> emptyPlains = emptyPlainsQuery.ToEntityArray(Allocator.TempJob);
            
            // Loop over all idle farmers, assigning a create farm task
            Entities.WithName("assign_till_task").
                WithAll<Farmer>().
                WithNone<DropOffCropTask>().
                WithNone<PickUpCropTask>().
                WithNone<TillTask>().
                WithNone<ChopForestTask>().
                WithNone<ChoppingTask>().
                WithReadOnly(emptyPlains).
                WithDisposeOnCompletion(emptyPlains).
                ForEach(
                    (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
                    {
                        // Find nearest empty plain
                        float minDistSq = float.MaxValue;
                        Entity nearestTarget = Entity.Null;
                        float2 nearestTargetPos = float2.zero;
                        for(int i = 0; i < emptyPlains.Length; i++)
                        {
                            float2 targetPos = GetComponent<Position>(emptyPlains[i]).Value;
                            float distSq = math.distancesq(targetPos, farmerPos.Value);
                            if(minDistSq > distSq && distSq < gameState.MaximumTaskDistance)
                            {
                                minDistSq = distSq;
                                nearestTarget = emptyPlains[i];
                                nearestTargetPos = targetPos;
                            }
                        }

                        if(nearestTarget != Entity.Null)
                        {
                            ecbWriter.AddComponent<TillTask>(entityInQueryIndex, farmerEntity);
                            ecbWriter.AddComponent(entityInQueryIndex, farmerEntity,
                                new TargetEntity { target = nearestTarget, targetPosition = nearestTargetPos });
                            ecbWriter.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                        }

                    }).ScheduleParallel();
            // Complete task assignment and structural changes
            Dependency.Complete();
            ecb.Playback(EntityManager);
            
        }
        
        if(nextTask <= 2)
        {
            NativeArray<Entity> forests = forestsQuery.ToEntityArray(Allocator.TempJob);

            // Loop over all idle farmers, assigning a chop forest task
            Entities.WithName("assign_chop_forest_task").
                WithAll<Farmer>().
                WithNone<DropOffCropTask>().
                WithNone<PickUpCropTask>().
                WithNone<TillTask>().
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
                        for(int i = 0; i < forests.Length; i++)
                        {
                            Translation forestTranslation = GetComponent<Translation>(forests[i]);
                            float2 forestPos = new float2(forestTranslation.Value.x, forestTranslation.Value.z);
                            float distSq = math.distancesq(forestPos, farmerPos.Value);
                            if(minDistSq > distSq)
                            {
                                minDistSq = distSq;
                                nearestForestEntity = forests[i];
                                nearestForestPos = forestPos;
                            }
                        }

                        if(nearestForestEntity != Entity.Null)
                        {
                            ecbWriter.AddComponent<ChopForestTask>(entityInQueryIndex, farmerEntity);
                            ecbWriter.AddComponent(entityInQueryIndex, farmerEntity,
                                new TargetEntity { target = nearestForestEntity, targetPosition = nearestForestPos });
                            ecbWriter.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                        }

                    }).ScheduleParallel();

            // Complete task assignment and structural changes
            Dependency.Complete();
            ecb.Playback(EntityManager);
        }

        ecb.Dispose();
    }

}
