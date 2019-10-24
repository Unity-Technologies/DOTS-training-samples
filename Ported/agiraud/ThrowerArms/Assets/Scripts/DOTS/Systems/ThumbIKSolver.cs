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
    struct ThumbIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, HandAxis>
    {
        public float Time;
        
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis)
        {
            // Arm + 4 fingers.
            var bufferOffset = 3 + 4 * 4 - 1;
            
            NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
            for (int i = 0; i < 4; i++)
            {
                chainPositions[i] = boneJoints[i + bufferOffset].JointPos;
            }

            float fingerGrabT = 1.0f;
//            if (heldRock!=null) {
//                // move our held rock to match our new hand position
//                heldRock.position = handMatrix.MultiplyPoint3x4(heldRockOffset);
//                lastIntendedRockPos = heldRock.position;
//
//                // if we're holding a rock, we're always gripping
//                fingerGrabT = 1f;
//            }
            
            var thumbPos = boneJoints[2].JointPos + handAxis.Right * thumbXOffset;
            var thumbTarget = thumbPos - handAxis.Right * .15f + handAxis.Forward * (.2f+.1f*fingerGrabT) - handAxis.Up*.1f;
            thumbTarget += handAxis.Right * math.sin(Time*3f + .5f) * .1f*(1f-fingerGrabT);
            
            // thumb bends away from the palm, instead of "upward" like the fingers
            Vector3 thumbBendHint = -handAxis.Right - handAxis.Forward * .5f;
            
            ConstantManager.IKSolve(ref chainPositions, thumbBoneLength,thumbPos, thumbTarget,
                thumbBendHint*thumbBendStrength);
            
            for (int i = 0; i < 4; i++)
            {
                boneJoints[i + bufferOffset] = new BoneJoint
                {
                    JointPos = chainPositions[i]
                };
            }
            
            chainPositions.Dispose();
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new ThumbIKSolverJob
        {
            Time = UnityEngine.Time.time // + timeOffset
        };
        return job.Schedule(this, inputDeps);
    }
}
