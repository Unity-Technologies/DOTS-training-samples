using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(IKSolverGroupSystem))]
[UpdateAfter(typeof(ArmIKSolver))]
public class ThumbIKSolver : JobComponentSystem
{
    private const float thumbBoneLength = 0.13f;
    private const float thumbBendStrength = 0.1f;
    private const float thumbXOffset = -0.05f;

    [BurstCompile]
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, HandAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis)
        {
            NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
            for (int i = 0; i < 4; i++)
            {
                chainPositions[i] = boneJoints[i + 2 + 4*3].JointPos;
            }

            var thumbPos = boneJoints[2].JointPos+handAxis.Right*thumbXOffset;
            var thumbTarget = thumbPos - handAxis.Right * .15f + handAxis.Forward - handAxis.Up*.1f;
            
            // thumb bends away from the palm, instead of "upward" like the fingers
            Vector3 thumbBendHint = (-handAxis.Right - handAxis.Forward * .5f);
            
            ConstantManager.IKSolve(ref chainPositions, thumbBoneLength,thumbPos, thumbTarget,
                thumbBendHint*thumbBendStrength);
            
            for (int i = 0; i < 3; i++)
            {
                boneJoints[i + 2 + 4*3] = new BoneJoint
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
