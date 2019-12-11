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
        var positions = EntityManager.GetBuffer<ArmJointPositionBuffer>(bufferSingleton);

        return Entities.WithName("UpdateArmIKChain")
            .ForEach((in ArmComponent arm, in Translation translation) =>
        {
            int lastIndex = (int) (translation.Value.x * ArmConstants.ChainCount + (ArmConstants.ChainCount - 1));
            positions[lastIndex] = arm.HandTarget;

            int firstIndex = (int) (translation.Value.x * ArmConstants.ChainCount);

            for (int i = lastIndex - 1; i >= firstIndex; i--)
            {
                positions[i] += arm.HandUp * ArmConstants.BendStrength;
                float3 delta = positions[i].Value - positions[i + 1].Value;
                positions[i] = positions[i + 1] + math.normalize(delta) * ArmConstants.BoneLength;
            }

            positions[firstIndex] = translation.Value;

            for (int i = firstIndex + 1; i <= lastIndex; i++)
            {
                float3 delta = positions[i].Value - positions[i - 1].Value;
                positions[i] = positions[i - 1] + math.normalize(delta) * ArmConstants.BoneLength;
            }
        }).Schedule(inputDeps);

//            FABRIK.Solve(armChain,armBoneLength,transform.position,handTarget,handUp*armBendStrength);
//            public static void Solve(Vector3[] chain, float boneLength, Vector3 anchor, Vector3 target, Vector3 bendHint) {
//                chain[chain.Length - 1] = target;
//                for (int i=chain.Length-2;i>=0;i--) {
//                    chain[i] += bendHint;
//                    Vector3 delta = chain[i] - chain[i + 1];
//                    chain[i] = chain[i + 1] + delta.normalized * boneLength;
//                }
//
//                chain[0] = anchor;
//                for (int i = 1; i<chain.Length; i++) {
//                    Vector3 delta = chain[i] - chain[i - 1];
//                    chain[i] = chain[i - 1] + delta.normalized * boneLength;
//                }
//            }
    }
}