using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UpdateArmIKChainSystem : JobComponentSystem
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
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var armJointPositionBuffer =
            EntityManager.GetBuffer<ArmJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity());
        var upVectorBufferForArmsAndFingers =
            EntityManager.GetBuffer<UpVectorBufferForArmsAndFingers>(m_handUpBufferQuery.GetSingletonEntity());
        
        JobHandle updateIkJob = Entities.WithName("UpdateArmIKChain")
            .WithNativeDisableParallelForRestriction(armJointPositionBuffer)
            .ForEach((in ArmComponent arm, in Translation translation) =>
        {
            int lastIndex = (int) (translation.Value.x * ArmConstants.ChainCount + (ArmConstants.ChainCount - 1));
            armJointPositionBuffer[lastIndex] = arm.HandTarget;

            int firstIndex = (int) (translation.Value.x * ArmConstants.ChainCount);

            for (int i = lastIndex - 1; i >= firstIndex; i--)
            {
                armJointPositionBuffer[i] += arm.HandUp * ArmConstants.BendStrength;
                float3 delta = armJointPositionBuffer[i].Value - armJointPositionBuffer[i + 1].Value;
                armJointPositionBuffer[i] = armJointPositionBuffer[i + 1] + math.normalizesafe(delta) * ArmConstants.BoneLength;
            }

            armJointPositionBuffer[firstIndex] = translation.Value;

            for (int i = firstIndex + 1; i <= lastIndex; i++)
            {
                float3 delta = armJointPositionBuffer[i].Value - armJointPositionBuffer[i - 1].Value;
                armJointPositionBuffer[i] = armJointPositionBuffer[i - 1] + math.normalizesafe(delta) * ArmConstants.BoneLength;
            }
        }).Schedule(inputDeps);

        float3 vRight = new float3(1.0f, 0.0f, 0.0f);

        JobHandle calculateHandMatrixJob = Entities.WithName("CalculateHandMatrix")
            .WithReadOnly(armJointPositionBuffer)
            .WithNativeDisableParallelForRestriction(upVectorBufferForArmsAndFingers)
            .ForEach((ref HandMatrix handMatrix, ref ArmComponent arm, in Translation translation) =>
            {
                int lastIndex = (int)(translation.Value.x * ArmConstants.ChainCount + (ArmConstants.ChainCount - 1));
                float3 armChainPosLast = armJointPositionBuffer[lastIndex].Value;
                float3 armChainPosBeforeLast = armJointPositionBuffer[lastIndex - 1].Value;

                arm.HandPosition = armChainPosLast;
                arm.HandForward = math.normalizesafe(armChainPosLast - armChainPosBeforeLast);
                arm.HandUp = math.normalizesafe(math.cross(arm.HandForward, vRight));
                upVectorBufferForArmsAndFingers[(int)translation.Value.x] = arm.HandUp;
                
                arm.HandRight = math.normalizesafe(math.cross(arm.HandForward, arm.HandUp));

                handMatrix.Value = new float4x4(math.RigidTransform(quaternion.LookRotationSafe(arm.HandForward, arm.HandUp), armChainPosLast));
            }).Schedule(updateIkJob);
        calculateHandMatrixJob.Complete();

        return calculateHandMatrixJob;
    }
}
