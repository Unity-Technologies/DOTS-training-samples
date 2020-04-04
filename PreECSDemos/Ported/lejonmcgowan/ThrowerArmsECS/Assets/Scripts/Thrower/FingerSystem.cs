using System;
using Unity.Entities;
using Unity.Mathematics;

[UpdateAfter(typeof(ArmSystem))]
public class FingerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var UpBases = GetComponentDataFromEntity<ArmUpComponentData>(true);
        var RightBases = GetComponentDataFromEntity<ArmRightComponentData>(true);
        var ForwardBases = GetComponentDataFromEntity<ArmForwardComponentData>(true);

        var ArmJointsFromEntity = GetBufferFromEntity<ArmJointElementData>(true);
        var ArmGrabTs = GetComponentDataFromEntity<ArmGrabTimerComponentData>(true);
        
        //float dt = Time.DeltaTime;
        float t = (float)Time.ElapsedTime;

        var grabCopyJob = Entities
            .WithReadOnly(ArmGrabTs)
            .ForEach((ref FingerGrabTimerComponentData fingerGrabT,
            in FingerParentComponentData fingerParent) =>
        {
            Entity armParent = fingerParent.armParentEntity;
            float armGrabT = ArmGrabTs[armParent];
            fingerGrabT = armGrabT;
        }).ScheduleParallel(Dependency);
        
        var IKJob = Entities
            .WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .ForEach((
                ref DynamicBuffer<FingerJointElementData> fingerJoints,
                in FingerParentComponentData fingerParent,
                in FingerIndexComponentData fingerIndex,
                in FingerGrabTimerComponentData fingerGrabT,
                in FingerThicknessComponentData fingerThickness) =>
            {
                Entity armParentEntity = fingerParent.armParentEntity;
                var armJointData = ArmJointsFromEntity[armParentEntity];
                var wristPos = armJointData[armJointData.Length - 1].value;

                float3 armUp = UpBases[armParentEntity];
                float3 armForward = ForwardBases[armParentEntity];
                float3 armRight = RightBases[armParentEntity];
                
                var fingerOffsetX = -0.12f;
                var fingerSpacing = 0.08f;
                
               //get base targetPosition
                float3 fingerPos = wristPos + armRight * (fingerOffsetX + fingerIndex * fingerSpacing);
                float3 fingerTarget = fingerPos + armForward * (.5f - .1f * fingerGrabT);
                //finger wiggle
                fingerTarget += .2f * armUp * math.sin((t + fingerIndex * .2f) * 3f)  * (1f - fingerGrabT);
                
                //perform rock position logic to lerp towards
                float3 debugRockPos = new float3(1, 0, 1.5f);
                float debugRadius = 0.25f;
                
                float3 rockFingerDelta = fingerTarget - debugRockPos;
                float3 rockFingerPos = debugRockPos +
                                        math.normalize(rockFingerDelta)  * (debugRadius * .5f + fingerThickness);


                fingerTarget = math.lerp(fingerTarget, rockFingerPos, fingerGrabT);
                
                //todo add arm spread during throw here, before IK
                
                
                //todo add in variable bonelength for each finger in a component
                FABRIK.Solve(fingerJoints.AsNativeArray().Reinterpret<float3>(), 0.2f, fingerPos, fingerTarget,
                    0.1f * armUp);
                
            }).ScheduleParallel(grabCopyJob);
        
        var thumbIKJob = Entities
            .WithNone<FingerIndexComponentData>()
            .WithReadOnly(ArmJointsFromEntity)
            .WithReadOnly(UpBases)
            .WithReadOnly(ForwardBases)
            .WithReadOnly(RightBases)
            .ForEach((
                ref DynamicBuffer<FingerJointElementData> thumbJoints,
                in FingerParentComponentData thumbParent,
                in FingerGrabTimerComponentData grabT,
                in FingerThicknessComponentData thickness) =>
            {
                Entity armParentEntity = thumbParent.armParentEntity;
                var armJointData = ArmJointsFromEntity[armParentEntity];
                var wristPos = armJointData[armJointData.Length - 1].value;

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
               
               float3 debugRockPos = new float3(1, 0, 1.5f);
               float debugRadius = 0.25f;
                
               float3 rockThumbDelta = thumbTarget - debugRockPos;
               float3 rockThumbPos = debugRockPos +
                                      math.normalize(rockThumbDelta)  * (debugRadius * .5f + thickness);


               thumbTarget = math.lerp(thumbTarget, rockThumbPos, grabT);
               
                FABRIK.Solve(thumbJoints.AsNativeArray().Reinterpret<float3>(), 0.13f, thumbPos, thumbTarget,
                    0.1f * thumbBendHint);
            }).ScheduleParallel(IKJob);

       
        Dependency = thumbIKJob;
    }
}