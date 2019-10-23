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
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, UpAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref UpAxis upAxis)
        {
            NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
            for (int i = 0; i < 4; i++)
            {
                chainPositions[i] = boneJoints[i + 2 + 4*3].JointPos;
            }

            var handForward = math.normalize(boneJoints[1].JointPos - boneJoints[0].JointPos);
            var handRight = math.cross(upAxis.Value, handForward);
            
            var thumbPos = boneJoints[2].JointPos+handRight*thumbXOffset;
            var thumbTarget = thumbPos - handRight * .15f + handForward - upAxis.Value*.1f;
            
            // thumb bends away from the palm, instead of "upward" like the fingers
            Vector3 thumbBendHint = (-handRight - handForward * .5f);
            
            ConstantManager.IKSolve(ref chainPositions, thumbBoneLength,chainPositions[0] /*TODO*/, armTarget.Value,
                upAxis.Value*thumbBendStrength);
            
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
