using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
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

        var ArmJointsFromEntity = GetBufferFromEntity<JointElementData>(true);
        var WristsFromEntity = GetComponentDataFromEntity<Wrist>(true);
        var ArmGrabTs = GetComponentDataFromEntity<ArmGrabTimer>(true);
        var ArmRockRecords = GetComponentDataFromEntity<ArmLastRockRecord>(true);
        var ArmThrowRecords = GetComponentDataFromEntity<ArmLastThrowRecord>(true);

        //float dt = Time.DeltaTime;
        float t = (float) Time.ElapsedTime;

        var grabCopyECB = beginSimECBSystem.CreateCommandBuffer().ToConcurrent();
        var grabCheckECB = beginSimECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
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
                    grabCopyECB.AddComponent<FingerGrabbedTag>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        Entities
            .WithName("FingerCheckGrabbed")
            .WithAny<FingerGrabbedTag>()
            .ForEach((Entity entity, int entityInQueryIndex, in FingerParent fingerParent) =>
            {
                if (!HasComponent<ArmReservedRock>(fingerParent))
                {
                    grabCheckECB.RemoveComponent<FingerGrabbedTag>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        //todo make use of a branch to combine this and ThumbIKJob
        Entities
            .WithName("FingerIKJob")
            .WithReadOnly(WristsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .WithReadOnly(ArmRockRecords)
            .WithReadOnly(ArmThrowRecords)
            .ForEach((
                ref DynamicBuffer<JointElementData> fingerJoints,
                in FingerParent fingerParent,
                in FingerIndex fingerIndex,
                in FingerGrabTimer fingerGrabT,
                in Thickness fingerThickness,
                in FingerLength fingerLength) =>
            {
                Entity armParentEntity = fingerParent.armParentEntity;
                var wristPos = GetComponent<Translation>(WristsFromEntity[armParentEntity].value).Value;
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
                    math.lerp(fingerTarget, rockFingerPos * (rockData.grabbing ? 1 : 0),
                        fingerGrabT); // fingerGrabT is already clamped [0..1]

                if (HasComponent<ArmLastThrowRecord>(armParentEntity))
                {
                    var openPalm = AnimUtils.EvauateThrowCurveSmooth(ArmThrowRecords[armParentEntity].throwTimer);

                    fingerTarget += (armUp * .3f + armForward * .1f + armRight * (fingerIndex - 1.5f) * .1f) * openPalm;
                }

                //todo add in variable bonelength for each finger in a component
                FABRIK.Solve(fingerJoints.AsNativeArray().Reinterpret<float3>(), fingerLength, fingerPos, fingerTarget,
                    0.2f * armUp);
            }).ScheduleParallel();

        Entities
            .WithName("ThumbIKJob")
            .WithNone<FingerIndex>()
            .WithReadOnly(WristsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .WithReadOnly(ArmRockRecords)
            .ForEach((
                ref DynamicBuffer<JointElementData> thumbJoints,
                in FingerParent thumbParent,
                in FingerGrabTimer grabT,
                in Thickness thickness) =>
            {
                Entity armParentEntity = thumbParent.armParentEntity;
                var wristPos = GetComponent<Translation>(WristsFromEntity[armParentEntity].value).Value;
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
            }).ScheduleParallel();
    }
}