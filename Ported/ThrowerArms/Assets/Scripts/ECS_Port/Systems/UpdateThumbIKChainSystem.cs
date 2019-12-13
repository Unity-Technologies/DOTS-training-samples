using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(UpdateFingerIKChainSystem))]
public class UpdateThumbIKChainSystem : JobComponentSystem
{
    private EntityQuery m_positionBufferQuery;
    private EntityQuery m_handUpBufferQuery;


    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_positionBufferQuery = 
            GetEntityQuery(ComponentType.ReadWrite<ArmJointPositionBuffer>());
        m_handUpBufferQuery = GetEntityQuery(ComponentType.ReadWrite<UpVectorBufferForArmsAndFingers>());

    }

    static bool IsNan(float3 f)
    {
        return float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float time = UnityEngine.Time.time;
        
        var thumbJointPositionBuffer =
            EntityManager.GetBuffer<ThumbJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity());
        var upVectorBufferForThumbs =
            EntityManager.GetBuffer<UpVectorBufferForThumbs>(m_handUpBufferQuery.GetSingletonEntity());

        ComponentDataFromEntity<Translation> translationFromEntityAccessor = GetComponentDataFromEntity<Translation>(isReadOnly:true);
        
        var calculateThumbIkWhenReachingJob = Entities.WithName("UpdateThumbIKWhenReachingForRock")
            .WithReadOnly(translationFromEntityAccessor)
            .WithNativeDisableParallelForRestriction(thumbJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForThumbs)
            .WithoutBurst()
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent, in ReachForTargetState reachTarget) =>
            {
                float3 thumbPosition = arm.HandPosition + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                var targetRockPosition = translationFromEntityAccessor[reachTarget.TargetEntity].Value;
                float3 rockThumbDelta = thumbTarget - targetRockPosition;
                float3 rockThumbPosition = targetRockPosition + math.normalize(rockThumbDelta) * (reachTarget.TargetSize * 0.5f);

                thumbTarget = math.lerp(thumbTarget, rockThumbPosition, fingerComponent.GrabExtent);

                // Solve this finger's IK chain
                int lastIndex = (int)(translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                int firstIndex = (int)(translation.Value.x * ThumbConstants.ChainCount);
                FABRIK_ECS.Solve(thumbJointPositionBuffer.Reinterpret<float3>(), firstIndex, lastIndex, ThumbConstants.BoneLength
                    , thumbPosition, thumbTarget, thumbBendHint);                
            }).Schedule(inputDeps);
        
         var calculateThumbIkWhenInThrowStateJob = Entities.WithName("UpdateThumbIKWhenInThrowState")
            .WithReadOnly(translationFromEntityAccessor)
            .WithoutBurst()
            .WithNativeDisableParallelForRestriction(thumbJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForThumbs)
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent, in HoldingRockState holdingRockState) =>
            {
                float3 thumbPosition = arm.HandPosition + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                if (!translationFromEntityAccessor.Exists(holdingRockState.HeldEntity))
                {
                    return;
                }
                var targetRockPosition = translationFromEntityAccessor[holdingRockState.HeldEntity].Value;
                float3 rockThumbDelta = thumbTarget - targetRockPosition;
                float3 rockThumbPosition = targetRockPosition + math.normalize(rockThumbDelta) * (holdingRockState.EntitySize * 0.5f);

                thumbTarget = math.lerp(thumbTarget, rockThumbPosition, fingerComponent.GrabExtent);

                // Solve this finger's IK chain
                int lastIndex = (int)(translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                int firstIndex = (int)(translation.Value.x * ThumbConstants.ChainCount);
                FABRIK_ECS.Solve(thumbJointPositionBuffer.Reinterpret<float3>(), firstIndex, lastIndex, ThumbConstants.BoneLength
                    , thumbPosition, thumbTarget, thumbBendHint);
            }).Schedule(calculateThumbIkWhenReachingJob);
        
        var calculateThumbIkWhenNotHoldingOrReachingForRock =
            Entities.WithName("UpdateThumbIKWhenNOTGrippingRock")
            .WithNativeDisableParallelForRestriction(thumbJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForThumbs)
            .WithNone<HoldingRockState>()
            .WithoutBurst()
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent) =>
            {
                float3 thumbPosition = arm.HandPosition + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                // Solve this finger's IK chain
                int lastIndex = (int)(translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                int firstIndex = (int)(translation.Value.x * ThumbConstants.ChainCount);
                FABRIK_ECS.Solve(thumbJointPositionBuffer.Reinterpret<float3>(), firstIndex, lastIndex, ThumbConstants.BoneLength
                    , thumbPosition, thumbTarget, thumbBendHint);
            }).Schedule(calculateThumbIkWhenInThrowStateJob);
        calculateThumbIkWhenNotHoldingOrReachingForRock.Complete();


        return calculateThumbIkWhenNotHoldingOrReachingForRock;
    }
}
