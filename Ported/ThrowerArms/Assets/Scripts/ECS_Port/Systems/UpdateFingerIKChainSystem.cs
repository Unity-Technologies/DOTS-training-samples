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
                    .ForEach((ref Finger finger, in ReachForTargetState reach) =>
                    {
                        finger.GrabExtent = 3f * reach.ReachTimer * reach.ReachTimer - 2f * reach.ReachTimer * reach.ReachTimer * reach.ReachTimer;
                    })
                    .Schedule(inputDeps);
        JobHandle grabExtentJob2 =
            Entities.WithName("UpdateFingerGrabExtentForHoldingTargetArm")
            .WithAll<GrabbingState>()
                    .ForEach((ref Finger finger) =>
                    {
                        finger.GrabExtent = 1f;
                    })
                    .Schedule(grabExtentJob1);
        
        return Entities.WithName("UpdateFingerIKChain")
            .WithReadOnly(translationFromEntity)
            .WithNativeDisableParallelForRestriction(fingerJointPositions)
            .ForEach(
            (ref Finger fingerComponent, in ArmComponent armComponent, in ReachForTargetState reachComponent, in Translation translation) =>
        {
            Translation rockTargetTranslation = translationFromEntity[reachComponent.TargetEntity];
            
            for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
            {
                float3 fingerPosition =
                    translation.Value +
                    armComponent.HandRight *
                    (FingerConstants.XOffset + finger * FingerConstants.Spacing);
                float3 fingerTarget = fingerPosition + armComponent.HandForward * (0.5f - 0.1f * fingerComponent.GrabExtent) 
                                + armComponent.HandUp * math.sin((time + finger * 0.2f) * 3f) * 0.2f * (1f - fingerComponent.GrabExtent);

                float3 rockFingerDelta = fingerTarget - rockTargetTranslation.Value;
                float3 rockFingerPosition = rockTargetTranslation.Value + math.normalize(rockFingerDelta) *
                                            (rockTargetTranslation.Value * 0.5f + FingerConstants.BoneThickness);

                fingerTarget = math.lerp(fingerTarget, rockFingerPosition, fingerComponent.GrabExtent) +
                               (armComponent.HandUp * 0.3f + armComponent.HandForward * 0.1f +
                                armComponent.HandRight * (finger - 1.5f) * 0.1f) * armComponent.OpenPalm;
                
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
    }
}