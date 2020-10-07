using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FakeTargetReachedSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();    
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        float m_ReachDistance = 0.05f;
        Entities
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref TargetEntity targetEntity,
                in Position position) =>
            {
                float2 targetPos = targetEntity.targetPosition;
                
                float distance = math.length(targetPos - position.Value);
                if(distance < m_ReachDistance)
                {
                    ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, entity);
                    ecb.DestroyEntity(entityInQueryIndex, targetEntity.target);
                }

            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}