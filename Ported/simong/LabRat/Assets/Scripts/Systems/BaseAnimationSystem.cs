using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BaseAnimationSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem m_EndSimulationECBS;

    protected override void OnCreate()
    {
        m_EndSimulationECBS = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;

        var ecb = m_EndSimulationECBS.CreateCommandBuffer();
        Entities.ForEach((Entity e, ref BaseAnimation anim, in AnimateBaseTag tag) =>
        {
            anim.CurrentTime = math.min(anim.CurrentTime + dt, anim.Time);
            float normalizedTime = anim.CurrentTime / anim.Time;

            //TODO: ???

            if (anim.CurrentTime >= anim.Time)
            {
                ecb.RemoveComponent<AnimateBaseTag>(e);
                anim.CurrentTime = 0;
            }
        }).Run();
    }
}
