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
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        var handoverThreshold = 0.01f;
        Entities
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
                }
                else
                {
                    ecb.AddComponent(entityInQueryIndex, entity, new Target{ Value = nextTranslation.Value });
                }
            }).ScheduleParallel();

        var ecb2 = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        
        // Filtering on RelayReturn here is necessary. Otherwise the query will trigger for the state the entities had at the beginning 
        // of this OnUpdate, so the Target from the above query haven't been added yet, and will get added below while there's an
        // add Target component pending from the above query.

        Entities
            .WithNone<Target>()
            .WithNone<RelayReturn>()
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
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

        // Extinguish
        Entities
            .WithAll<FirefighterFullLastTag>()
            .ForEach((ref WaterBucketID waterBucketID) =>
        {
            var waterBucket = GetComponent<WaterBucket>(waterBucketID.Value);
            waterBucket.Value = 0.0f;
            SetComponent(waterBucketID.Value, waterBucket);
            SetComponent(waterBucketID.Value, new BaseColor { Value = new float4(255.0f/255.0f, 105.0f/255.0f, 117.0f/255.0f, 1.0f) });
        }).Schedule();
        
        // Refill
        Entities
            .WithAll<FirefighterEmptyLastTag>()
            .ForEach((ref WaterBucketID waterBucketID) =>
            {
                var waterBucket = GetComponent<WaterBucket>(waterBucketID.Value);
                waterBucket.Value = 1.0f;
                SetComponent(waterBucketID.Value, waterBucket);
                SetComponent(waterBucketID.Value, new BaseColor { Value = new float4(0.0f/255.0f, 250.0f/255.0f, 255.0f/255.0f, 1.0f) });
            }).Schedule();
    }
}
