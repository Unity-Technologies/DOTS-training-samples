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
    private const float fingerBoneLength = 0.5f;
    private const float fingerBendStrength = 0.2f;
    
    private const float fingerXOffset = -0.12f;
    private const float fingerSpacing = 0.08f;

    [BurstCompile]
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, HandAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref HandAxis handAxis)
        {

            for (var j = 0; j < 4; j++)
            {
                NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
                for (var i = 0; i < 4; i++)
                {
                    chainPositions[i] = boneJoints[i + j * 4 + 2].JointPos;
                }
                
                // find knuckle position for this finger
                var fingerPos = boneJoints[2].JointPos + handAxis.Right * (fingerXOffset + j * fingerSpacing);

                // find resting position for this fingertip
                var fingerTarget = fingerPos; // + handFwd * (.5f-.1f*fingerGrabT);

                fingerTarget += handAxis.Up * .3f + handAxis.Forward * .1f + handAxis.Right * (j - 1.5f) * .1f;
                
                ConstantManager.IKSolve(ref chainPositions, fingerBoneLength,fingerPos, fingerTarget,
                    handAxis.Up*fingerBendStrength);
            
                for (int i = 0; i < 4; i++)
                {
                    boneJoints[i + j * 4 + 3] = new BoneJoint
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
