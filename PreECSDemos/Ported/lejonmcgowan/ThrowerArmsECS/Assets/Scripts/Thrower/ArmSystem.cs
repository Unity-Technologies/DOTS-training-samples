using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

[UpdateBefore(typeof(ArmRenderUpdateSystem))]
public class ArmSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float t = (float)Time.ElapsedTime;
        
        var idleJob = Entities.ForEach((ref ArmIdleTargetComponentData armIdleTarget,
            in AnchorPosComponentData anchorPos) =>
        {
            float time = t + anchorPos.value.x + anchorPos.value.y + anchorPos.value.z;
            
            armIdleTarget = anchorPos +
                             new float3(math.sin(time) * .35f, 1f + math.cos(time * 1.618f) * .5f, 1.5f);

        }).ScheduleParallel(Dependency);

        // This will eventually be a lerp between idle target and grab target
        //JobHandle lerpInputJobs = JobHandle.CombineDependencies(idleJob, grabJob)
        var ikTargetJob = Entities.ForEach((ref ArmIKTargetComponentData armIKTarget,
            in ArmIdleTargetComponentData armIdleTarget) =>
        {
            armIKTarget.value = armIdleTarget;
        }).ScheduleParallel(idleJob);
        
        var armIkJob = Entities
            .ForEach((
                ref ArmForwardComponentData armForward,
                ref ArmRightComponentData armRight,
                ref ArmUpComponentData armUp,
                ref DynamicBuffer<ArmJointElementData> armJoints,
                in ArmIKTargetComponentData armTarget,
                in AnchorPosComponentData armAnchorPos
               ) =>
            {
                FABRIK.Solve(armJoints.AsNativeArray().Reinterpret<float3>(), 1f, armAnchorPos.value, armTarget.value,
                    0.1f * armUp.value);
                
                float3 transformRight = new float3(1, 0, 0);
                armForward.value =
                    math.normalize(armJoints[armJoints.Length - 1].value - armJoints[armJoints.Length - 2].value);
                armUp.value = math.normalize(math.cross(armForward.value, transformRight));
                armRight.value = math.cross(armUp.value, armForward.value);
            }).ScheduleParallel(ikTargetJob);

        // Last job becomes the new output dependency
        Dependency = armIkJob;
    }
}