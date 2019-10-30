using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using GameAI;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class RockSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var dt = Time.deltaTime;
        var Cmd = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
     
        Entities.ForEach((Entity entity, int nativeThreadIndex, ref RockSmashSpeed s, ref RockComponent rock, ref HealthComponent health) =>
        {
            health.Value -= s.Value * dt;

            if (health.Value < 0.0)
            {
                // remove entity
                Cmd.DestroyEntity(nativeThreadIndex, entity);
            }
        }).Schedule(inputDeps);
        
        return inputDeps;
    }
}