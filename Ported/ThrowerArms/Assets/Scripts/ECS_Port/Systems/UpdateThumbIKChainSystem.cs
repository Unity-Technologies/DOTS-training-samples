using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateThumbIKChainSystem : JobComponentSystem
{
    private EntityQuery m_positionBufferQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_positionBufferQuery = 
            GetEntityQuery(ComponentType.ReadWrite<ArmJointPositionBuffer>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float time = UnityEngine.Time.time;
        
        Entity bufferSingleton = m_positionBufferQuery.GetSingletonEntity();
        var positions = EntityManager.GetBuffer<ArmJointPositionBuffer>(bufferSingleton);

        ComponentDataFromEntity<Translation> translationFromEntityAccessor = GetComponentDataFromEntity<Translation>(isReadOnly:true);

        return Entities.WithName("UpdateThumbIK")
            .WithReadOnly(translationFromEntityAccessor)
            .WithNativeDisableParallelForRestriction(positions)
            .ForEach((in ArmComponent arm, in Translation translation, in Finger fingerComponent, in ReachForTargetState reachTarget) =>
            {
                float3 thumbPosition = translation.Value + arm.HandRight * ThumbConstants.XOffset;
                float3 thumbTarget = thumbPosition - arm.HandRight * 0.15f +
                                     arm.HandForward * (0.2f - 0.1f * fingerComponent.GrabExtent) - arm.HandUp * 0.1f;
                thumbTarget += arm.HandRight * math.sin(time * 3f + 0.5f) * 0.1f * (1f - fingerComponent.GrabExtent);

                float3 thumbBendHint = -arm.HandRight - arm.HandForward * 0.5f;

                var targetRockPosition = translationFromEntityAccessor[reachTarget.TargetEntity].Value;
                float3 rockThumbDelta = thumbTarget - targetRockPosition;
                float3 rockThumbPosition = targetRockPosition + math.normalize(rockThumbDelta) * (reachTarget.TargetSize * 0.5f);

                thumbTarget = math.lerp(thumbTarget, rockThumbPosition, fingerComponent.GrabExtent);
                
                int lastIndex = (int) (translation.Value.x * ThumbConstants.ChainCount + (ThumbConstants.ChainCount - 1));
                positions[lastIndex] = thumbTarget;

                int firstIndex = (int) (translation.Value.x * ThumbConstants.ChainCount);

                for (int i = lastIndex - 1; i >= firstIndex; i--)
                {
                    positions[i] += thumbBendHint;
                    float3 delta = positions[i].Value - positions[i + 1].Value;
                    positions[i] = positions[i + 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }

                positions[firstIndex] = translation.Value;

                for (int i = firstIndex + 1; i <= lastIndex; i++)
                {
                    float3 delta = positions[i].Value - positions[i - 1].Value;
                    positions[i] = positions[i - 1] + math.normalize(delta) * ThumbConstants.BoneLength;
                }
            }).Schedule(inputDeps);
    }
}
