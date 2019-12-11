using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateArmIKChainSystem : JobComponentSystem
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
        Entity bufferSingleton = m_positionBufferQuery.GetSingletonEntity();
        var armJointPositions = EntityManager.GetBuffer<ArmJointPositionBuffer>(bufferSingleton);

        return Entities.WithName("UpdateArmIKChain")
            .ForEach((in ArmComponent arm, in Translation translation) =>
        {
            int lastIndex = (int) (translation.Value.x * ArmConstants.ChainCount + (ArmConstants.ChainCount - 1));
            armJointPositions[lastIndex] = arm.HandTarget;

            int firstIndex = (int) (translation.Value.x * ArmConstants.ChainCount);

            for (int i = lastIndex - 1; i >= firstIndex; i--)
            {
                armJointPositions[i] += arm.HandUp * ArmConstants.BendStrength;
                float3 delta = armJointPositions[i].Value - armJointPositions[i + 1].Value;
                armJointPositions[i] = armJointPositions[i + 1] + math.normalize(delta) * ArmConstants.BoneLength;
            }

            armJointPositions[firstIndex] = translation.Value;

            for (int i = firstIndex + 1; i <= lastIndex; i++)
            {
                float3 delta = armJointPositions[i].Value - armJointPositions[i - 1].Value;
                armJointPositions[i] = armJointPositions[i - 1] + math.normalize(delta) * ArmConstants.BoneLength;
            }
        }).Schedule(inputDeps);
    }
}