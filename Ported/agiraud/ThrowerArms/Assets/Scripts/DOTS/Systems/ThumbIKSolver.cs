using System.ComponentModel;
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

    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct ThumbIKSolverJob : IJobForEach_BCC<BoneJoint, ArmTarget, HandAxis>
    {
        public float Time;
        
        public void Execute(DynamicBuffer<BoneJoint> boneJoints,
            [Unity.Collections.ReadOnly] ref ArmTarget armTarget, [Unity.Collections.ReadOnly] ref HandAxis handAxis)
        {
            // Arm + 4 fingers.
            var bufferOffset = 3 + 4 * 4 - 1;
            
            NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
            for (int i = 0; i < 4; i++)
            {
                chainPositions[i] = boneJoints[i + bufferOffset].JointPos;
            }

            var fingerGrabT = 1; //timer.GrabT;
            if (armTarget.IsHolding)
                fingerGrabT = 1; // -> Gripping position

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
