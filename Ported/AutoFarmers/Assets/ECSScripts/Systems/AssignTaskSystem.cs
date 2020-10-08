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

    EntityQuery cropsQuery;
    EntityQuery emptyPlainsQuery;

    protected override void OnCreate()
    {
        m_Random = new Random(666);

        cropsQuery = GetEntityQuery(typeof(Crop), ComponentType.Exclude<Assigned>());
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
        NativeArray<Entity> crops = cropsQuery.ToEntityArray(Allocator.TempJob);

        // Loop over all idle farmers, assigning a pickup crop task
        Entities.
            WithName("assign_pickup_crop_task").
            WithAll<Farmer>().
            WithNone<DropOffCropTask>().
            WithNone<PickUpCropTask>().
            WithNone<TillTask>().
            WithReadOnly(crops).
            WithDisposeOnCompletion(crops).
            ForEach(
            (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
            {
                // Find nearest crop
                float minDistSq = float.MaxValue;
                Entity nearestTarget = Entity.Null;
                float2 nearestTargetPos = float2.zero;
                for (int i = 0; i < crops.Length; i++)
                {
                    Translation cropTranslation = GetComponent<Translation>(crops[i]);
                    float2 cropPos = new float2(cropTranslation.Value.x, cropTranslation.Value.z);
                    float distSq = math.distancesq(cropPos, farmerPos.Value);
                    if (minDistSq > distSq && distSq < gameState.MaximumTaskDistance)
                    {
                        minDistSq = distSq;
                        nearestTarget = crops[i];
                        nearestTargetPos = cropPos;
                    }
                }

                if (nearestTarget != Entity.Null)
                {
                    ecbWriter.AddComponent<PickUpCropTask>(entityInQueryIndex, farmerEntity);
                    ecbWriter.AddComponent<TargetEntity>(entityInQueryIndex, farmerEntity);
                    ecbWriter.SetComponent(entityInQueryIndex, farmerEntity, new TargetEntity { target = nearestTarget, targetPosition = nearestTargetPos });
                    ecbWriter.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                }

            }).ScheduleParallel();

        // Complete task assignment and structural changes
        Dependency.Complete();
        ecb.Playback(EntityManager);

        // Loop over all idle farmers, assigning a create farm task
        NativeArray<Entity> emptyPlains = emptyPlainsQuery.ToEntityArray(Allocator.TempJob);
        Entities.
            WithName("assign_till_task").
            WithAll<Farmer>().
            WithNone<DropOffCropTask>().
            WithNone<PickUpCropTask>().
            WithNone<TillTask>().
            WithReadOnly(emptyPlains).
            WithDisposeOnCompletion(emptyPlains).
            ForEach(
            (Entity farmerEntity, int entityInQueryIndex, in Position farmerPos) =>
            {
                // Find nearest empty plain
                float minDistSq = float.MaxValue;
                Entity nearestTarget = Entity.Null;
                float2 nearestTargetPos = float2.zero;
                for (int i = 0; i < emptyPlains.Length; i++)
                {
                    float2 targetPos = GetComponent<Position>(emptyPlains[i]).Value;
                    float distSq = math.distancesq(targetPos, farmerPos.Value);
                    if (minDistSq > distSq && distSq < gameState.MaximumTaskDistance)
                    {
                        minDistSq = distSq;
                        nearestTarget = emptyPlains[i];
                        nearestTargetPos = targetPos;
                    }
                }

                if (nearestTarget != Entity.Null)
                {
                    ecbWriter.AddComponent<TillTask>(entityInQueryIndex, farmerEntity);
                    ecbWriter.AddComponent<TargetEntity>(entityInQueryIndex, farmerEntity);
                    ecbWriter.SetComponent(entityInQueryIndex, farmerEntity, new TargetEntity { target = nearestTarget, targetPosition = nearestTargetPos });
                    ecbWriter.AddComponent<NeedsDeduplication>(entityInQueryIndex, farmerEntity);
                }

            }).ScheduleParallel();

        // Complete task assignment and structural changes
        Dependency.Complete();
        ecb.Playback(EntityManager);

        ecb.Dispose();
    }

}
