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
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, HandAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis)
        {
            for (var j = 0; j < fingerCount; j++)
            {
                NativeArray<float3> chainPositions = new NativeArray<float3>(fingerJointCount, Allocator.Temp);
                for (var i = 0; i < fingerJointCount; i++)
                {
                    chainPositions[i] = boneJoints[i + j * fingerCount + armOffset].JointPos;
                }
                
                // find knuckle position for this finger
                var fingerPos = boneJoints[armOffset].JointPos + handAxis.Right * (fingerXOffset + j * fingerSpacing);

                // find resting position for this fingertip (fingerTarget)
                var reachTimer = 0f; // TODO add timer stuff
                // FYI if we're holding a rock, we're always gripping
                // fingerGrabT = 1 -> Gripping position
                var grabT = reachTimer;
                grabT = 3f * grabT * grabT - 2f * grabT * grabT * grabT;
                var fingerGrabT = grabT;

                var fingerTarget = fingerPos + handAxis.Forward * (.5f-.1f*fingerGrabT);

                // TODO: add spooky fingers
                // fingerTarget += handUp * Mathf.Sin((time + i*.2f)*3f) * .2f*(1f-fingerGrabT);

                // apply finger-spreading during throw animation
                float openPalm = 0f; // TODO throwCurve.Evaluate(throwTimer);
                fingerTarget += (handAxis.Up * .3f + handAxis.Forward * .1f + handAxis.Right * (j - 1.5f) * .1f) * openPalm ;

                ConstantManager.IKSolve(ref chainPositions, fingerBoneLength[j],fingerPos, fingerTarget,
                    handAxis.Up*fingerBendStrength);
            
                for (int i = 0; i < fingerJointCount; i++)
                {
                    boneJoints[i + j * fingerCount + armOffset] = new BoneJoint
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
        return new ArmIKSolverJob().Schedule(this, inputDeps);
    }
}
