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

    private static float2 CalculatePosition(float positionInLine, float2 from, float2 fromTo, float2 normal)
    {
        float offset = 0.4f * positionInLine * (1.0f - positionInLine);
        return fromTo * positionInLine + from + offset * normal;
    }
    
    protected override void OnUpdate()
    {
        float2 dst = float2.zero;
        using (var poiEntities = m_PointOfInterestEvaluatedQuery.ToEntityArray(Allocator.TempJob))
        {
            if (poiEntities.Length > 0)
            {
                var poiEntity = poiEntities[poiEntities.Length - 1];
                dst = GetComponent<PointOfInterestEvaluated>(poiEntity).POIPoisition;
            }

            Debug.Log("POI evaluated picked up by form line: " + dst);
            EntityManager.RemoveComponent<PointOfInterestEvaluated>(m_PointOfInterestEvaluatedQuery);
        }
        
        float2 src = new float2(0.0f, 7.0f);
        float2 fromTo = (dst - src);
        float2 normal = new float2(-fromTo.y, fromTo.x);
        
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<Target>()
            .WithNone<WaterBucketID>()
            .ForEach((int entityInQueryIndex, Entity entity, FirefighterFullTag firefighter, FirefighterPositionInLine positionInLine, in Translation2D translation) =>
        {
            float2 pos = CalculatePosition(positionInLine.Value, src, fromTo, normal);
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
            float2 pos = CalculatePosition(positionInLine.Value, src, fromTo, normal);
            ecb.AddComponent<Target>(entityInQueryIndex, entity, new Target{ Value = pos });
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
