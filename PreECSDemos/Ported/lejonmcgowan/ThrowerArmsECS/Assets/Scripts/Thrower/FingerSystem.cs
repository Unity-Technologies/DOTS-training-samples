using System;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(ArmSystem))]
public class FingerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var UpBases = GetComponentDataFromEntity<ArmUpComponentData>(true);
        
        //float dt = Time.DeltaTime;
        float t = (float)Time.ElapsedTime;

        // spooky finger wiggling while we're idle 
        var idleMvoeJob = Entities.WithReadOnly(UpBases).ForEach((ref FingerIKTargetComponentData fingerTarget,
            ref FingerGrabTimerComponentData grabT,
            in FingerParentComponentData fingerParent,
            in FingerIndexComponentData fingerIndex) =>
        {
            Entity armParentEntity = fingerParent.armParentEntity;
            float3 armUp = UpBases[armParentEntity];
            fingerTarget = .2f * armUp * math.sin((t + fingerIndex * .2f) * 3f)  * (1f - grabT);
        }).ScheduleParallel(Dependency);

        
        
        var RightBases = GetComponentDataFromEntity<ArmRightComponentData>(true);
        var ArmJointsFromEntity = GetBufferFromEntity<ArmJointElementData>(true);

        var IKJob = Entities
            .WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(RightBases)
            .ForEach((
                ref DynamicBuffer<FingerJointElementData> fingerJoints,
                ref FingerIKTargetComponentData fingerTarget,
                in FingerParentComponentData fingerParent,
                in FingerIndexComponentData fingerIndex) =>
            {
                Entity armParentEntity = fingerParent.armParentEntity;
                var armJointData = ArmJointsFromEntity[armParentEntity];
                var wristPos = armJointData[armJointData.Length - 1].value;

                float3 armUp = UpBases[armParentEntity];
                float3 armRight = RightBases[armParentEntity];
                var fingerOffsetX = -0.12f;
                var fingerSpacing = 0.08f;

                float3 fingerPos = wristPos + armRight * (fingerOffsetX + fingerIndex * fingerSpacing);

                fingerTarget += fingerPos + new float3(0f, 0.3f, 0.55f);

                FABRIK.Solve(fingerJoints.AsNativeArray().Reinterpret<float3>(), 0.2f, fingerPos, fingerTarget,
                    0.1f * armUp);
            }).ScheduleParallel(idleMvoeJob);

       
        Dependency = IKJob;
    }
}