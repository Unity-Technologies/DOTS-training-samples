using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class AnimateEntitySystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameStartedTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var dt = Time.DeltaTime;

        Dependency = Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Scale scale, ref BounceScaleAnimationProperties scaleAnimationProperties) =>
            {
                if (scaleAnimationProperties.AccumulatedTime + dt > scaleAnimationProperties.AnimationDuration)
                {
                    scale.Value = scaleAnimationProperties.OriginalScale;
                    ecb.RemoveComponent<BounceScaleAnimationProperties>(entityInQueryIndex,entity);
                }
                else
                {
                    float halfAnimDuration = scaleAnimationProperties.AnimationDuration / 2.0f;
                    scaleAnimationProperties.AccumulatedTime += dt;
                    if (scaleAnimationProperties.AccumulatedTime <= halfAnimDuration)
                    {
                        scale.Value = Unity.Mathematics.math.lerp(scaleAnimationProperties.OriginalScale, scaleAnimationProperties.TargetScale, scaleAnimationProperties.AccumulatedTime/halfAnimDuration );
                    }
                    else
                    {
                        scale.Value = math.lerp(scaleAnimationProperties.TargetScale, scaleAnimationProperties.OriginalScale, 1.0f - (scaleAnimationProperties.AnimationDuration - scaleAnimationProperties.AccumulatedTime)/halfAnimDuration);
                    }
                }

            }).ScheduleParallel(Dependency);
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
