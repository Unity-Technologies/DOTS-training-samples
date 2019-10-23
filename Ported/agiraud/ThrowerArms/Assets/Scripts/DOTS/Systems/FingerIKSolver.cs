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
    struct ArmIKSolverJob : IJobForEachWithEntity_EBCC<BoneJoint, ArmTarget, UpAxis>
    {
        public void Execute(Entity entity, int index, DynamicBuffer<BoneJoint> boneJoints,
            [ReadOnly] ref ArmTarget armTarget, [ReadOnly] ref UpAxis upAxis)
        {

            for (var j = 0; j < 4; j++)
            {
                NativeArray<float3> chainPositions = new NativeArray<float3>(4, Allocator.Temp);
                for (var i = 0; i < 4; i++)
                {
                    chainPositions[i] = boneJoints[i + j * 4 + 3].JointPos;
                }

                var handForward = math.normalize(boneJoints[1].JointPos - boneJoints[0].JointPos);
                var handRight = math.cross(upAxis.Value, handForward);
                // find knuckle position for this finger
                var fingerPos = boneJoints[2].JointPos + handRight * (fingerXOffset + j * fingerSpacing);

                // find resting position for this fingertip
                var fingerTarget = fingerPos; // + handFwd * (.5f-.1f*fingerGrabT);

                fingerTarget += upAxis.Value * .3f + handForward * .1f + handRight * (j - 1.5f) * .1f;
                
                ConstantManager.IKSolve(ref chainPositions, fingerBoneLength,fingerPos, fingerTarget,
                    upAxis.Value*fingerBendStrength);
            
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
