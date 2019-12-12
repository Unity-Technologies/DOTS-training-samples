using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ArmWindupSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
    
        var accessor = GetComponentDataFromEntity<Translation>(true);
        EntityCommandBuffer.Concurrent concurrentBuffer = entityCommandBuffer.ToConcurrent();
        var dt = Time.DeltaTime;
        var deps = Entities
            .WithReadOnly(accessor)
            .ForEach((Entity entity, int entityInQueryIndex, ref ArmComponent arm, ref WindingUpState windup, in Translation pos) =>
            {
                windup.WindupTimer += dt / ArmConstants.WindUpDuration;
                if (windup.WindupTimer < 0f)
                    return;
                float windupT = Mathf.Clamp01(windup.WindupTimer);
                windupT = 3f * windupT * windupT - 2f * windupT * windupT * windupT;
                arm.HandTarget = Vector3.Lerp(arm.HandTarget, windup.HandTarget, windupT);
                if (!accessor.Exists(windup.AimedTargetEntity))
                {
                    concurrentBuffer.RemoveComponent<WindingUpState>(entityInQueryIndex, entity);
                    concurrentBuffer.AddComponent<LookForThrowTargetState>(entityInQueryIndex, entity, new LookForThrowTargetState { GrabbedEntity = windup.HeldEntity, TargetSize = 1.0f } );
                    return;
                }
                var flatTargetDelta = accessor[windup.AimedTargetEntity].Value - pos.Value;
                flatTargetDelta.y = 0f;
                flatTargetDelta = math.normalize(flatTargetDelta);
                // windup position is "behind us," relative to the target position
                windup.HandTarget = pos.Value - flatTargetDelta * 2f + new float3(0,1,0) * (3f - windupT * 2.5f);
         //       concurrentBuffer.SetComponent(0, windup.HeldEntity, new Translation { Value = arm.HandTarget });
                if(windup.WindupTimer > 1f)
                {
                    concurrentBuffer.RemoveComponent<WindingUpState>(0, entity);
                    concurrentBuffer.AddComponent(0,entity, new ThrowAtState
                    {
                        StartPosition = windup.HandTarget,
                        AimedTargetEntity = windup.AimedTargetEntity,
                        HeldEntity = windup.HeldEntity
                    });
                }
            })
            .Schedule(inputDeps);
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(deps);
        return deps;
    }

}
