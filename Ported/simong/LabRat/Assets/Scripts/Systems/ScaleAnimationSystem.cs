using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ScaleAnimationSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationECBS;

    protected override void OnCreate()
    {
        m_EndSimulationECBS = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;

        var ecb = m_EndSimulationECBS.CreateCommandBuffer().ToConcurrent();

        // check for special case
        // entities already animating but with a new request
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref ScaleAnimation animation, in ScaleRequest request) =>
            {
                animation.Timer += request.Time;
                ecb.RemoveComponent<ScaleRequest>(entityInQueryIndex, entity);
            })
            .WithName("ProcessExistingScaleAnimationRequests")
            .ScheduleParallel();

        // process requests
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, in ScaleRequest request) =>
            {
                ecb.SetComponent(entityInQueryIndex, entity, new NonUniformScale { Value = new float3(request.Scale) });
                ecb.AddComponent(entityInQueryIndex, entity, new ScaleAnimation { Timer = request.Time });
                ecb.RemoveComponent<ScaleRequest>(entityInQueryIndex, entity);
            })
            .WithName("ProcessNewScaleAnmationRequests")
            .ScheduleParallel();

        // process animation
        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref ScaleAnimation animation) =>
            {
                animation.Timer -= dt;
                if (animation.Timer <= 0f)
                {
                    ecb.SetComponent(entityInQueryIndex, entity, new NonUniformScale { Value = new float3(1f) });
                    ecb.RemoveComponent<ScaleAnimation>(entityInQueryIndex, entity);
                }
            })
            .WithName("ProcessScaleAnmations")
            .ScheduleParallel();

        m_EndSimulationECBS.AddJobHandleForProducer(Dependency);
    }
}
