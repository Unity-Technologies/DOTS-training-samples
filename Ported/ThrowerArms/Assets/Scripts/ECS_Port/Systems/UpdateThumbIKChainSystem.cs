using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UpdateThumbIKChainSystem : JobComponentSystem
{
    private EntityQuery m_positionBufferQuery;
    private EntityQuery m_matrixBufferQuery;
    private EntityQuery m_handUpBufferQuery;


    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_positionBufferQuery = 
            GetEntityQuery(ComponentType.ReadWrite<ArmJointPositionBuffer>());
        m_matrixBufferQuery =
            GetEntityQuery(ComponentType.ReadWrite<ArmJointMatrixBuffer>());
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
        var thumbJointMatriceBuffer =
            EntityManager.GetBuffer<ThumbJointMatrixBuffer>(m_matrixBufferQuery.GetSingletonEntity());
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
                float3 thumbPosition = translation.Value + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                var targetRockPosition = translationFromEntityAccessor[reachTarget.TargetEntity].Value;
                float3 rockThumbDelta = thumbTarget - targetRockPosition;
                float3 rockThumbPosition = targetRockPosition + math.normalize(rockThumbDelta) * (reachTarget.TargetSize * 0.5f);

                thumbTarget = math.lerp(thumbTarget, rockThumbPosition, fingerComponent.GrabExtent);
                
                int lastIndex = (int) (translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                thumbJointPositionBuffer[lastIndex] = thumbTarget;

                int firstIndex = (int) (translation.Value.x * ThumbConstants.ChainCount);

                for (int i = lastIndex - 1; i >= firstIndex; i--)
                {
                    thumbJointPositionBuffer[i] += thumbBendHint;
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i + 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i + 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }

                thumbJointPositionBuffer[firstIndex] = translation.Value;

                for (int i = firstIndex + 1; i <= lastIndex; i++)
                {
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i - 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i - 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }
            }).Schedule(inputDeps);
        
         var calculateThumbIkWhenInThrowStateJob = Entities.WithName("UpdateThumbIKWhenInThrowState")
            .WithReadOnly(translationFromEntityAccessor)
            .WithoutBurst()
            .WithNativeDisableParallelForRestriction(thumbJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForThumbs)
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent, in LookForThrowTargetState throwState) =>
            {
                float3 thumbPosition = translation.Value + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                if (!translationFromEntityAccessor.Exists(throwState.GrabbedEntity))
                {
                    return;
                }
                var targetRockPosition = translationFromEntityAccessor[throwState.GrabbedEntity].Value;
                float3 rockThumbDelta = thumbTarget - targetRockPosition;
                float3 rockThumbPosition = targetRockPosition + math.normalize(rockThumbDelta) * (throwState.TargetSize * 0.5f);

                thumbTarget = math.lerp(thumbTarget, rockThumbPosition, fingerComponent.GrabExtent);
                
                int lastIndex = (int) (translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                thumbJointPositionBuffer[lastIndex] = thumbTarget;

                int firstIndex = (int) (translation.Value.x * ThumbConstants.ChainCount);

                for (int i = lastIndex - 1; i >= firstIndex; i--)
                {
                    thumbJointPositionBuffer[i] += thumbBendHint;
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i + 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i + 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }

                thumbJointPositionBuffer[firstIndex] = translation.Value;

                for (int i = firstIndex + 1; i <= lastIndex; i++)
                {
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i - 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i - 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }
            }).Schedule(calculateThumbIkWhenReachingJob);
        
        var calculateThumbIkWhenNotHoldingOrReachingForRock =
            Entities.WithName("UpdateThumbIKWhenNOTGrippingRock")
            .WithNativeDisableParallelForRestriction(thumbJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForThumbs)
            .WithNone<HoldingRockState>()
            .WithoutBurst()
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent) =>
            {
                float3 thumbPosition = translation.Value + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;
                upVectorBufferForThumbs[(int) translation.Value.x] = thumbBendHint;

                // Solve thumb IK chain
                int lastIndex = (int) (translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                thumbJointPositionBuffer[lastIndex] = thumbTarget;

                int firstIndex = (int) (translation.Value.x * ThumbConstants.ChainCount);

                for (int i = lastIndex - 1; i >= firstIndex; i--)
                {
                    thumbJointPositionBuffer[i] += thumbBendHint;
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i + 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i + 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }

                thumbJointPositionBuffer[firstIndex] = translation.Value;

                for (int i = firstIndex + 1; i <= lastIndex; i++)
                {
                    float3 delta = thumbJointPositionBuffer[i].Value - thumbJointPositionBuffer[i - 1].Value;
                    thumbJointPositionBuffer[i] = thumbJointPositionBuffer[i - 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }
            }).Schedule(calculateThumbIkWhenInThrowStateJob);
        calculateThumbIkWhenNotHoldingOrReachingForRock.Complete();


        return
            new UpdateBoneMatrixJob
            {
                BoneTranslations = thumbJointPositionBuffer.AsNativeArray().Reinterpret<float3>(),
                UpVectorsForMatrixCalculations = upVectorBufferForThumbs.AsNativeArray().Reinterpret<float3>(),
                NumBoneTranslationsPerArm = ThumbConstants.ChainCount,
                BoneThickness = ArmConstants.BoneThickness,
                
                BoneMatrices = thumbJointMatriceBuffer.AsNativeArray().Reinterpret<Matrix4x4>()
            }.Schedule(
                thumbJointPositionBuffer.Length - 1, 
                innerloopBatchCount: 256,
                dependsOn: calculateThumbIkWhenNotHoldingOrReachingForRock);
    }
}
