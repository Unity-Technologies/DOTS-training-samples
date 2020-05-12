using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor.SceneManagement;

[UpdateAfter(typeof(ArmIKSystem))]
public class FingerSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimECBSystem;

    protected override void OnCreate()
    {
        beginSimECBSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var UpBases = GetComponentDataFromEntity<ArmBasesUp>(true);
        var RightBases = GetComponentDataFromEntity<ArmBasesRight>(true);
        var ForwardBases = GetComponentDataFromEntity<ArmBasesForward>(true);

        var ArmJointsFromEntity = GetBufferFromEntity<ArmJointElementData>(true);
        var ArmGrabTs = GetComponentDataFromEntity<ArmGrabTimer>(true);
        var ArmRockRecords = GetComponentDataFromEntity<ArmLastRockRecord>(true);
        var ArmThrowRecords = GetComponentDataFromEntity<ArmLastThrowRecord>(true);

        //float dt = Time.DeltaTime;
        float t = (float) Time.ElapsedTime;

        var ecb = beginSimECBSystem.CreateCommandBuffer().ToConcurrent();

        var fingerGrabTCopyJob = Entities
            .WithName("FingerGrabTCopyJob")
            .WithNone<FingerGrabbedTag>()
            .WithReadOnly(ArmGrabTs)
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref FingerGrabTimer fingerGrabT,
                in FingerParent fingerParent) =>
            {
                Entity armParent = fingerParent.armParentEntity;
                float armGrabT = ArmGrabTs[armParent];
                fingerGrabT = armGrabT;

                if (fingerGrabT >= 1f)
                {
                    ecb.AddComponent<FingerGrabbedTag>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);


        //todo see if possible to disable native safety checks on DynamicBuffer parameter of foreach Job
        var fingerIKJob = Entities
            .WithName("FingerIKJob")
            .WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .WithReadOnly(ArmRockRecords)
            .WithReadOnly(ArmThrowRecords)
            .ForEach((
                ref DynamicBuffer<FingerJointElementData> fingerJoints,
                in FingerParent fingerParent,
                in FingerIndex fingerIndex,
                in FingerGrabTimer fingerGrabT,
                in FingerThickness fingerThickness,
                in FingerLength fingerLength) =>
            {
                Entity armParentEntity = fingerParent.armParentEntity;
                var armJointData = ArmJointsFromEntity[armParentEntity];
                var wristPos = armJointData[armJointData.Length - 1].value;
                var rockData = ArmRockRecords[armParentEntity];

                float3 armUp = UpBases[armParentEntity];
                float3 armForward = ForwardBases[armParentEntity];
                float3 armRight = RightBases[armParentEntity];

                var fingerOffsetX = -0.12f;
                var fingerSpacing = 0.08f;


                //get base targetPosition
                float3 fingerPos = wristPos + armRight * (fingerOffsetX + fingerIndex * fingerSpacing);
                float3 fingerTarget = fingerPos + armForward * (.5f - .1f * fingerGrabT);
                //finger wiggle
                fingerTarget += .2f * armUp * math.sin((t + fingerIndex * .2f) * 3f) * (1f - fingerGrabT);

                float3 rockFingerDelta = fingerTarget - rockData.pos;
                float3 rockFingerPos = rockData.pos +
                                       math.normalize(rockFingerDelta) * (rockData.size * 0.5f + fingerThickness);


                fingerTarget =
                    math.lerp(fingerTarget, rockFingerPos, fingerGrabT); // fingerGrabT is already clamped [0..1]

                if (HasComponent<ArmLastThrowRecord>(armParentEntity))
                {
                    var openPalm = AnimUtils.EvauateThrowCurveSmooth(ArmThrowRecords[armParentEntity].throwTimer);

                    fingerTarget += (armUp * .3f + armForward * .1f + armRight * (fingerIndex - 1.5f) * .1f) * openPalm;
                }

                //todo add in variable bonelength for each finger in a component
                FABRIK.Solve(fingerJoints.AsNativeArray().Reinterpret<float3>(), fingerLength, fingerPos, fingerTarget,
                    0.2f * armUp);
            }).ScheduleParallel(fingerGrabTCopyJob);

        var thumbIKJob = Entities
            .WithName("ThumbIKJob")
            .WithNone<FingerIndex>()
            .WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .WithReadOnly(ArmRockRecords)
            .ForEach((
                ref DynamicBuffer<FingerJointElementData> thumbJoints,
                in FingerParent thumbParent,
                in FingerGrabTimer grabT,
                in FingerThickness thickness) =>
            {
                Entity armParentEntity = thumbParent.armParentEntity;
                var armJointData = ArmJointsFromEntity[armParentEntity];
                var wristPos = armJointData[armJointData.Length - 1].value;
                var rockData = ArmRockRecords[armParentEntity];

                float3 armUp = UpBases[armParentEntity];
                float3 armForward = ForwardBases[armParentEntity];
                float3 armRight = RightBases[armParentEntity];

                var thumbOffsetX = -0.08f;

                //get base targetPosition
                float3 thumbPos = wristPos + armRight * thumbOffsetX;
                float3 thumbBendHint = -armRight - armForward * .5f;

                float3 thumbTarget = .1f * armRight * math.sin(t * 3f + .5f) * (1f - grabT);
                //thumb wiggle?
                thumbTarget += thumbPos - armRight * .15f + armForward * (.2f + .1f * grabT) - armUp * .1f;

                float3 rockThumbDelta = thumbTarget - rockData.pos;
                float3 rockThumbPos = rockData.pos +
                                      math.normalize(rockThumbDelta) * (rockData.size * .5f + thickness);


                thumbTarget = math.lerp(thumbTarget, rockThumbPos, grabT);

                FABRIK.Solve(thumbJoints.AsNativeArray().Reinterpret<float3>(), 0.13f, thumbPos, thumbTarget,
                    0.1f * thumbBendHint);
            }).ScheduleParallel(fingerIKJob);

        Dependency = JobHandle.CombineDependencies(fingerIKJob, thumbIKJob);
    }
}