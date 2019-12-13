using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(UpdateArmIKChainSystem))]
public class UpdateFingerIKChainSystem : JobComponentSystem
{
    private EntityQuery m_positionBufferQuery;
    private EntityQuery m_handUpBufferQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_positionBufferQuery = 
            GetEntityQuery(ComponentType.ReadOnly<FingerJointPositionBuffer>());
        m_handUpBufferQuery = GetEntityQuery(ComponentType.ReadWrite<UpVectorBufferForArmsAndFingers>());
    }

    static bool IsNan(float3 f)
    {
        return float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var fingerJointPositions = 
            EntityManager.GetBuffer<FingerJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity());
        var upVectorBufferForArmsAndFingers =
            EntityManager.GetBuffer<UpVectorBufferForArmsAndFingers>(m_handUpBufferQuery.GetSingletonEntity());


        float time = UnityEngine.Time.time + TimeConstants.Offset;
        var translationFromEntity = GetComponentDataFromEntity<Translation>(true);
        
        JobHandle grabExtentJob1 = 
            Entities.WithName("UpdateFingerGrabExtentForReachingTargetArm")
                    .WithNone<HoldingRockState>()
                    .ForEach((ref Finger finger, in ArmComponent arm) =>
                    {
                        finger.GrabExtent = 3f * arm.ReachTimer * arm.ReachTimer - 2f * arm.ReachTimer * arm.ReachTimer * arm.ReachTimer;
                    })
                    .Schedule(inputDeps);
        
        JobHandle grabExtentJob2 =
            Entities.WithName("UpdateFingerGrabExtentForHoldingTargetArm")
                    .WithAll<HoldingRockState>()
                    .ForEach((ref Finger finger, in ArmComponent arm) =>
                    {
                        finger.GrabExtent = 1f;
                    })
                    .Schedule(grabExtentJob1);

        JobHandle updateFingerIkChainForFingersGrippingRock = 
            Entities.WithName("UpdateFingerIKChainForFingersGrippingRock")
            .WithReadOnly(translationFromEntity)
            .WithNativeDisableParallelForRestriction(fingerJointPositions)
            .ForEach(
                (ref Finger fingerComponent,
                    in ArmComponent armComponent, 
                    in LookForThrowTargetState lookForThrowTargetState, 
                    in Translation translation) =>
                {
                    if (!translationFromEntity.Exists(lookForThrowTargetState.GrabbedEntity))
                    {
                        return;
                    }
                    Translation rockTargetTranslation = translationFromEntity[lookForThrowTargetState.GrabbedEntity];
            
                    for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
                    {
                        // Find knuckle position for this finger
                        float3 fingerPosition =
                            armComponent.HandPosition +
                            armComponent.HandRight *
                            (FingerConstants.XOffset + finger * FingerConstants.Spacing);
                        
                        // Find resting position for this fingertip
                        float3 fingerTarget = fingerPosition + armComponent.HandForward * (0.5f - 0.1f * fingerComponent.GrabExtent); 
                        // Spooky finger wriggling while we are idle
                        fingerTarget += armComponent.HandUp * math.sin((time + finger * 0.2f) * 3f) * 0.2f * (1f - fingerComponent.GrabExtent);

                        // Since we are gripping, move this fingertip to the surface of our gripped rock
                        float3 rockFingerDelta = fingerTarget - rockTargetTranslation.Value;
                        float3 rockFingerPosition = rockTargetTranslation.Value + math.normalize(rockFingerDelta) *
                                                    (rockTargetTranslation.Value * 0.5f + FingerConstants.BoneThickness);
                        fingerTarget = math.lerp(fingerTarget, rockFingerPosition, fingerComponent.GrabExtent);
                        
                        // Apply finger-spreading during throw animation
                        fingerTarget += 
                            (armComponent.HandUp * 0.3f + armComponent.HandForward * 0.1f + armComponent.HandRight * (finger - 1.5f) * 0.1f) * ArmThrowSystem.GetCurveValue(armComponent.ThrowTimer);
            
                        // Solve this finger's IK chain
                        int lastIndex = (int) (translation.Value.x * FingerConstants.TotalChainCount + (finger * FingerConstants.PerFingerChainCount - 1));
                        int firstIndex = lastIndex - FingerConstants.PerFingerChainCount + 1;
                        FABRIK_ECS.Solve(fingerJointPositions.Reinterpret<float3>(), firstIndex, lastIndex, FingerConstants.BoneLengths[finger - 1]
                            , fingerPosition, fingerTarget, armComponent.HandUp * FingerConstants.BendStrength);
                    }
                }).Schedule(grabExtentJob2);
        
         var updateFingerIkChainForFingersIdle =
             Entities.WithName("UpdateFingerIKChainForFingersIdle")
             .WithNativeDisableParallelForRestriction(fingerJointPositions)
             .WithNone<LookForThrowTargetState>()
             .ForEach((ref Finger fingerComponent, in ArmComponent armComponent, in Translation translation) =>
             {
                 for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
                 {
                     // Find knuckle position for this finger
                     float3 fingerPosition = armComponent.HandPosition + armComponent.HandRight *
                                             (FingerConstants.XOffset + finger * FingerConstants.Spacing);

                     // Find resting position for this fingertip
                     float3 fingerTarget =
                         fingerPosition + armComponent.HandForward * (0.5f - 0.1f * fingerComponent.GrabExtent);

                     // spooky finger wiggling while we're idle
                     fingerTarget += armComponent.HandUp * math.sin((time + finger * 0.2f) * 3f) * 0.2f *
                                     (1f - fingerComponent.GrabExtent);

                     // Solve this finger's IK chain
                     int lastIndex = (int)(translation.Value.x * FingerConstants.TotalChainCount + (finger * FingerConstants.PerFingerChainCount - 1));
                     int firstIndex = lastIndex - FingerConstants.PerFingerChainCount + 1;
                     FABRIK_ECS.Solve(fingerJointPositions.Reinterpret<float3>(), firstIndex, lastIndex, FingerConstants.BoneLengths[finger - 1]
                         , fingerPosition, fingerTarget, armComponent.HandUp * FingerConstants.BendStrength);
                 }
             })
             .Schedule(updateFingerIkChainForFingersGrippingRock);

        return updateFingerIkChainForFingersIdle;
         
    }
}