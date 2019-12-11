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
        
        return Entities.WithName("UpdateFingerIKChain").ForEach((ref FingerComponent fingerComponent, in ArmComponent arm, in Translation translation) =>
        {
            fingerComponent.GrabExtent = 
                3f * arm.ReachTimer * arm.ReachTimer - 2f * arm.ReachTimer * arm.ReachTimer * arm.ReachTimer;
            
            // TODO: Check whether a rock is being held. If it is, then grabExtent should always be set to 1.
            
            for (int finger = 1; finger <= FingerConstants.CountPerArm; finger++)
            {
                float3 fingerPosition =
                    translation.Value +
                    arm.HandRight *
                    (FingerConstants.XOffset + finger * FingerConstants.Spacing);
                float3 fingerTarget = fingerPosition + arm.HandForward * (0.5f - 0.1f * fingerComponent.GrabExtent) 
                                + arm.HandUp * math.sin((time + finger * 0.2f) * 3f) * 0.2f * (1f - fingerComponent.GrabExtent);

                float3 rockFingerDelta = fingerTarget - arm.LastIntendedRockPosition;
                float3 rockFingerPosition = arm.LastIntendedRockPosition + math.normalize(rockFingerDelta) * (arm.LastIntendedRockPosition * 0.5f + FingerConstants.BoneThickness);
                
                fingerTarget = math.lerp(fingerTarget, rockFingerPosition, fingerComponent.GrabExtent) + ((arm.HandUp * 0.3f + arm.HandForward * 0.1f + arm.HandRight * (finger - 1.5f) * 0.1f) * arm.OpenPalm);
                
                int lastIndex = 
                    (int) (translation.Value.x * FingerConstants.TotalChainCount + (finger * FingerConstants.PerFingerChainCount - 1));
                
                fingerJointPositions[lastIndex] = fingerTarget;

                int firstIndex = lastIndex - FingerConstants.PerFingerChainCount + 1;

                for (int i = lastIndex - 1; i >= firstIndex; i--)
                {
                    fingerJointPositions[i] += arm.HandUp * FingerConstants.BendStrength;
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
        }).Schedule(inputDeps);
    }
}