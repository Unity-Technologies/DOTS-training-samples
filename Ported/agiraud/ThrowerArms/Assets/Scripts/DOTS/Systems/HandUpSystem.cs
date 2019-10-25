using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(IKSolverGroupSystem))]
public class HandUpSystem : JobComponentSystem
{
    [BurstCompile(FloatMode = FloatMode.Fast)]
    struct HandUpJob : IJobForEach_BC<BoneJoint, HandAxis>
    {
        public void Execute(DynamicBuffer<BoneJoint> boneJoints, ref HandAxis handAxis)
        {
            var handForward = new float3(0, 0, 1);
            if (math.abs(math.length(boneJoints[2].JointPos - boneJoints[1].JointPos)) > 0.000001f )
                handForward = math.normalize(boneJoints[2].JointPos - boneJoints[1].JointPos);

            var handUp = math.cross(handForward, new float3(1.0f, 0, 0));
            var handRight = math.cross(handUp, handForward);
            
            handAxis = new HandAxis
            {
                Up = handUp,
                Forward = handForward,
                Right = handRight
            };
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new HandUpJob().Schedule(this, inputDeps);
    }
}
