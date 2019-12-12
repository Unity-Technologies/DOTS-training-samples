using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateFingerIKChainSystem : JobComponentSystem
{
    private EntityQuery m_positionBufferQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_positionBufferQuery = 
            GetEntityQuery(ComponentType.ReadOnly<FingerJointPositionBuffer>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entity bufferSingleton = m_positionBufferQuery.GetSingletonEntity();
        var fingerJointPositions = EntityManager.GetBuffer<FingerJointPositionBuffer>(bufferSingleton);

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
                    .ForEach((ref Finger finger, ref ArmComponent arm) =>
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
                    Translation rockTargetTranslation = translationFromEntity[lookForThrowTargetState.GrabbedEntity];
            
                    for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
                    {
                        // Find knuckle position for this finger
                        float3 fingerPosition =
                            translation.Value +
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
                        int lastIndex = 
                            (int) (translation.Value.x * FingerConstants.TotalChainCount + (finger * FingerConstants.PerFingerChainCount - 1));
                
                        fingerJointPositions[lastIndex] = fingerTarget;

                        int firstIndex = lastIndex - FingerConstants.PerFingerChainCount + 1;

                        for (int i = lastIndex - 1; i >= firstIndex; i--)
                        {
                            fingerJointPositions[i] += armComponent.HandUp * FingerConstants.BendStrength;
                            float3 delta = fingerJointPositions[i].Value - fingerJointPositions[i + 1].Value;
                            fingerJointPositions[i] = fingerJointPositions[i + 1] + math.normalize(delta) * FingerConstants.BoneLengths[finger - 1];
                        }

                        fingerJointPositions[firstIndex] = fingerPosition;

                        for (int i = firstIndex + 1; i <= lastIndex; i++)
                        {
                            float3 delta = fingerJointPositions[i].Value - fingerJointPositions[i - 1].Value;
                            fingerJointPositions[i] = fingerJointPositions[i - 1] + math.normalize(delta) * FingerConstants.BoneLengths[finger - 1];
                        }
                    }
                }).Schedule(grabExtentJob2);
        
         return
             Entities.WithName("UpdateFingerIKChainForFingersGrippingRock")
             .WithReadOnly(translationFromEntity)
             .WithNativeDisableParallelForRestriction(fingerJointPositions)
             .WithNone<LookForThrowTargetState>()
             .ForEach((ref Finger fingerComponent, in ArmComponent armComponent, in Translation translation) =>
             {
                 for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
                 {
                     // Find knuckle position for this finger
                     float3 fingerPosition = translation.Value + armComponent.HandRight *
                                             (FingerConstants.XOffset + finger * FingerConstants.Spacing);

                     // Find resting position for this fingertip
                     float3 fingerTarget =
                         fingerPosition + armComponent.HandForward * (0.5f - 0.1f * fingerComponent.GrabExtent);

                     // spooky finger wiggling while we're idle
                     fingerTarget += armComponent.HandUp * math.sin((time + finger * 0.2f) * 3f) * 0.2f *
                                     (1f - fingerComponent.GrabExtent);

                     // Solve this finger's IK chain
                     int lastIndex = (int) (translation.Value.x * FingerConstants.TotalChainCount +
                                            (finger * FingerConstants.PerFingerChainCount - 1));

                     fingerJointPositions[lastIndex] = fingerTarget;

                     int firstIndex = lastIndex - FingerConstants.PerFingerChainCount + 1;

                     for (int i = lastIndex - 1; i >= firstIndex; i--)
                     {
                         fingerJointPositions[i] += armComponent.HandUp * FingerConstants.BendStrength;
                         float3 delta = fingerJointPositions[i].Value - fingerJointPositions[i + 1].Value;
                         fingerJointPositions[i] = fingerJointPositions[i + 1] +
                                                   math.normalize(delta) * FingerConstants.BoneLengths[finger - 1];
                     }

                     fingerJointPositions[firstIndex] = fingerPosition;

                     for (int i = firstIndex + 1; i <= lastIndex; i++)
                     {
                         float3 delta = fingerJointPositions[i].Value - fingerJointPositions[i - 1].Value;
                         fingerJointPositions[i] = fingerJointPositions[i - 1] +
                                                   math.normalize(delta) * FingerConstants.BoneLengths[finger - 1];
                     }
                 }
             })
             .Schedule(updateFingerIkChainForFingersGrippingRock);
    }
}