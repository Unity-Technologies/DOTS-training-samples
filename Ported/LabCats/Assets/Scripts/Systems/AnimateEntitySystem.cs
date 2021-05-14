using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(UpdateTransformSystem))]
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

        Entities
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

            }).ScheduleParallel();

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Rotation rotation, ref RotateAnimationProperties rotAnimationProperties) =>
            {
                if (rotAnimationProperties.AccumulatedTime + dt > rotAnimationProperties.AnimationDuration)
                {
                    rotation.Value = rotAnimationProperties.TargetRotation;
                    ecb.RemoveComponent<RotateAnimationProperties>(entityInQueryIndex,entity);
                }
                else
                {
                    rotAnimationProperties.AccumulatedTime += dt;
                    var mixedRotation = math.slerp(rotAnimationProperties.OriginalRotation, rotAnimationProperties.TargetRotation, rotAnimationProperties.AccumulatedTime / rotAnimationProperties.AnimationDuration);
                    rotation.Value = mixedRotation;
                }
            }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
