using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(IKSolverGroupSystem))]
[UpdateAfter(typeof(ArmIKSolver))]
public class FingerIKSolver : JobComponentSystem
{
    private static readonly float[] fingerBoneLength = {0.2f, 0.22f, 0.2f, 0.16f};

    private const float fingerBendStrength = 0.2f;
    
    private const float fingerXOffset = -0.12f;
    private const float fingerSpacing = 0.08f;
    // Hardcoded count / offset
    private const int fingerCount = 4;
    private const int fingerJointCount = 4;
    private const int armOffset = 2;

    [BurstCompile]
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCCC<BoneJoint, ArmTarget, HandAxis, Timers>
    {
        [ReadOnly] public float time;
        
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis, [ReadOnly] ref Timers timer)
        {
            for (var j = 0; j < fingerCount; j++)
            {
                NativeArray<float3> chainPositions = new NativeArray<float3>(fingerJointCount, Allocator.Temp);
                for (var i = 0; i < fingerJointCount; i++)
                {
                    chainPositions[i] = boneJoints[i + j * fingerJointCount + armOffset].JointPos;
                }
                
                // find knuckle position for this finger
                var fingerPos = boneJoints[armOffset].JointPos + handAxis.Right * (fingerXOffset + j * fingerSpacing);

                // find resting position for this fingertip (fingerTarget)
                var fingerGrabT = timer.GrabT;
                if (armTarget.IsHolding)
                    fingerGrabT = 1; // -> Gripping position

                var fingerTarget = fingerPos + handAxis.Forward * (.5f-.1f*fingerGrabT);

                // Spooky fingers
                fingerTarget += handAxis.Up * math.sin((time + j*.2f)*3f) * .2f*(1f-fingerGrabT);

                // apply finger-spreading during throw animation
                float openPalm = timer.Throw;
                fingerTarget += (handAxis.Up * .3f + handAxis.Forward * .1f + handAxis.Right * (j - 1.5f) * .1f) * openPalm ;

                ConstantManager.IKSolve(ref chainPositions, fingerBoneLength[j],fingerPos, fingerTarget,
                    handAxis.Up*fingerBendStrength);
            
                for (int i = 0; i < fingerJointCount; i++)
                {
                    boneJoints[i + j * fingerJointCount + armOffset] = new BoneJoint
                    {
                        JointPos = chainPositions[i]
                    };
                }
            
                chainPositions.Dispose();
            }


        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        ArmIKSolverJob job = new ArmIKSolverJob
        {
            time = Time.time
        };
        return job.Schedule(this, inputDeps);
    }
}
