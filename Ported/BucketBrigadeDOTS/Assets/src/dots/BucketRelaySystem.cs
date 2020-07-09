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
            .ForEach((int entityInQueryIndex
                , Entity entity
                , in FirefighterNext firefighterNext
                , in Translation2D translation2D
                , in WaterBucketID waterBucketId) =>
            {
                var next = firefighterNext.Value;
                Translation2D nextTranslation = GetComponent<Translation2D>(next);
                if (math.length(nextTranslation.Value - translation2D.Value) < handoverThreshold)
                {
                    ecb.AddComponent(entityInQueryIndex, next, waterBucketId);
                    ecb.RemoveComponent<WaterBucketID>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        Entities
            .WithNone<Target>()
            .WithAll<WaterBucketID>()
            .ForEach((int entityInQueryIndex
                , Entity entity
                , in FirefighterNext firefighterNext) =>
            {
                var next = firefighterNext.Value;
                Translation2D nextTranslation = GetComponent<Translation2D>(next);
                ecb.AddComponent(entityInQueryIndex, entity, new Target{ Value = nextTranslation.Value });
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
