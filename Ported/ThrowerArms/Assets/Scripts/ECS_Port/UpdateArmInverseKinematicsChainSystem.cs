using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class UpdateArmInverseKinematicsChainSystem : JobComponentSystem
{
    private EntityQuery m_inverseKinematcsDataCreatorQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        m_inverseKinematcsDataCreatorQuery = 
            GetEntityQuery(ComponentType.ReadOnly<ArmJointPositionBuffer>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entity ikDataSingleton = m_inverseKinematcsDataCreatorQuery.GetSingletonEntity();
        var armJointPositions = EntityManager.GetBuffer<ArmJointPositionBuffer>(ikDataSingleton);

        return Entities.ForEach((in ArmComponent arm, in Translation translation) =>
        {
            int lastIndex = (int)(translation.Value.x * ArmConstants.ArmChainCount + (ArmConstants.ArmChainCount - 1));
            armJointPositions[lastIndex] = arm.HandTarget;

            int firstIndex = (int) (translation.Value.x * ArmConstants.ArmChainCount);
            
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
        }).Schedule(inputDeps);
    }
}