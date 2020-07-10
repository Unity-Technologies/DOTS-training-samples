using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public class FirefighterFormLineSystem : SystemBase
{
    private EntityQuery m_PointOfInterestEvaluatedQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_PointOfInterestEvaluatedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                typeof(PointOfInterestEvaluated),
            }
        });
        
        RequireForUpdate(m_PointOfInterestEvaluatedQuery);
        
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {        
        Debug.Log("POI evaluated picked up by form line");

        Entity poi = Entity.Null;
        using (var poiEntities = m_PointOfInterestEvaluatedQuery.ToEntityArray(Allocator.TempJob))
        {
            Debug.Log("    POI evaluated picked up by form line: " + poiEntities.Length);
            if (poiEntities.Length == 0)
            {
                EntityManager.RemoveComponent<PointOfInterestEvaluated>(m_PointOfInterestEvaluatedQuery);
                return;
            }

            poi = poiEntities[poiEntities.Length - 1];
        }
        
        float2 src = new float2(0.0f, 7.0f);
        float2 dst = GetComponent<PointOfInterestEvaluated>(poi).POIPoisition;
        EntityManager.RemoveComponent<PointOfInterestEvaluated>(m_PointOfInterestEvaluatedQuery);
        float2 fromTo = (dst - src);
        float2 normal = new float2(-fromTo.y, fromTo.x);
        
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<Target>()
            .WithNone<WaterBucketID>()
            .ForEach((int entityInQueryIndex, Entity entity, FirefighterFullTag firefighter, FirefighterPositionInLine positionInLine, in Translation2D translation) =>
        {
            float offset = 0.4f * positionInLine.Value * (1.0f - positionInLine.Value);
            float2 pos = fromTo * positionInLine.Value + src + offset * normal;
            ecb.AddComponent<Target>(entityInQueryIndex, entity, new Target{ Value = pos });
        }).ScheduleParallel();

        fromTo = -fromTo;
        normal = -normal;
        float2 temp = src;
        src = dst;
        dst = temp;

        Entities
            .WithNone<Target>()
            .WithNone<WaterBucketID>()
            .ForEach((int entityInQueryIndex, Entity entity, FirefighterEmptyTag firefighter, FirefighterPositionInLine positionInLine, in Translation2D translation) =>
        {
            float offset = 0.4f * positionInLine.Value * (1.0f - positionInLine.Value);
            float2 pos = fromTo * positionInLine.Value + src + offset * normal;
            ecb.AddComponent<Target>(entityInQueryIndex, entity, new Target{ Value = pos });
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
