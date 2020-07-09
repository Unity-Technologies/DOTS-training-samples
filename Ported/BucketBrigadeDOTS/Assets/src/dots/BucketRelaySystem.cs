using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine.Assertions.Must;

public class BucketRelaySystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        // var ecbSeq = m_ECBSystem.CreateCommandBuffer();
        var ecbSeq = new EntityCommandBuffer(Allocator.TempJob);
        var ecb = ecbSeq.ToConcurrent();

        var handoverThreshold = 0.01f;
        var job = Entities
            // .WithStructuralChanges()
            .WithNone<Target>()
            .ForEach((int entityInQueryIndex
                , Entity entity
                , in FirefighterNext firefighterNext
                , in Translation2D translation2D
                , in WaterBucketID waterBucketId
                , in RelayReturn relayReturn) =>
            {
                var next = firefighterNext.Value;
                Translation2D nextTranslation = GetComponent<Translation2D>(next);
                if (math.length(nextTranslation.Value - translation2D.Value) < handoverThreshold)
                {
                    ecb.AddComponent(entityInQueryIndex, next, waterBucketId);
                    ecb.RemoveComponent<WaterBucketID>(entityInQueryIndex, entity);
                    ecb.AddComponent(entityInQueryIndex, entity, new Target{ Value = relayReturn.Value });
                    ecb.RemoveComponent<RelayReturn>(entityInQueryIndex, entity);

                    // ecbSeq.AddComponent(next, waterBucketId);
                    // ecbSeq.RemoveComponent<WaterBucketID>(entity);
                    // ecbSeq.AddComponent(entity, new Target{ Value = relayReturn.Value });
                    // ecbSeq.RemoveComponent<RelayReturn>(entity);

                    // EntityManager.AddComponentData(next, waterBucketId);
                    // EntityManager.RemoveComponent<WaterBucketID>(entity);
                    // EntityManager.AddComponentData(entity, new Target{ Value = relayReturn.Value });
                    // EntityManager.RemoveComponent<RelayReturn>(entity);
                }
            }).ScheduleParallel(Dependency);
            // }).Schedule();
            // }).Run();

        job.Complete();
        ecbSeq.Playback(EntityManager);
        ecbSeq.Dispose();

        var ecbSeq2 = m_ECBSystem.CreateCommandBuffer();
        var ecb2 = ecbSeq2.ToConcurrent();
        
        Entities
            // .WithStructuralChanges()
            .WithNone<Target>()
            .WithAll<WaterBucketID>()
            .ForEach((int entityInQueryIndex
                , Entity entity
                , in FirefighterNext firefighterNext
                , in Translation2D translation) =>
            {
                var next = firefighterNext.Value;
                Translation2D nextTranslation = GetComponent<Translation2D>(next);
                ecb2.AddComponent(entityInQueryIndex, entity, new Target{ Value = nextTranslation.Value });
                ecb2.AddComponent(entityInQueryIndex, entity, new RelayReturn{ Value = translation.Value });

                // ecbSeq.AddComponent(entity, new Target{ Value = nextTranslation.Value });
                // ecbSeq.AddComponent(entity, new RelayReturn{ Value = translation.Value });

                // EntityManager.AddComponentData(entity, new Target{ Value = nextTranslation.Value });
                // EntityManager.AddComponentData(entity, new RelayReturn{ Value = translation.Value });

            }).ScheduleParallel();
            // }).Run();

        // ecbSeq.Playback(EntityManager);

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
