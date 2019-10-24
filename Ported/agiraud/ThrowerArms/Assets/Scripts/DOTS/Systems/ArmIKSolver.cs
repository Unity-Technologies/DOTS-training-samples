using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class IKSolverGroupSystem : ComponentSystemGroup { }

[UpdateInGroup(typeof(IKSolverGroupSystem))]
[UpdateAfter(typeof(HandUpSystem))]
public class ArmIKSolver : JobComponentSystem
{
    private const float armBoneLength = 1;
    private const float armBendStrength = 0.1f;

    [BurstCompile]
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, HandAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis)
        {
            NativeArray<float3> chainPositions = new NativeArray<float3>(3, Allocator.Temp);
            for (int i = 0; i < 3; i++)
            {
                chainPositions[i] = boneJoints[i].JointPos;
            }

            ConstantManager.IKSolve(ref chainPositions, armBoneLength,chainPositions[0] /*TODO*/, armTarget.Value,
                handAxis.Up*armBendStrength);
            
            for (int i = 0; i < 3; i++)
            {
                boneJoints[i] = new BoneJoint
                {
                    JointPos = chainPositions[i]
                };
            }
            
            chainPositions.Dispose();
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new ArmIKSolverJob().Schedule(this, inputDeps);
    }
}
