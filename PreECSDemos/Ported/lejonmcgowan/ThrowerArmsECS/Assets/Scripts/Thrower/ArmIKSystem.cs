using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(ArmSetupSystem))]
[UpdateBefore(typeof(ArmRenderUpdateSystem))]
public class ArmIKSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_beginSimEcbSystem;

    private static float3 AimAtCan(float3 canPos, float3 startPos, float3 canVelocity)
    {
        // predictive aiming based on this article by Kain Shin:
        // https://www.gamasutra.com/blogs/KainShin/20090515/83954/Predictive_Aim_Mathematics_for_AI_Targeting.php

        float3 up = new float3(0, 1, 0);
        float3 forward = new float3(0, 0, 1);
        float baseThrowSpeed = 24f;

        float targetSpeed = math.length(canVelocity);
        float cosTheta = math.dot(math.normalize(startPos - canPos), math.normalize(canVelocity));
        float D = math.length(canPos - startPos);

        // quadratic equation terms
        float A = baseThrowSpeed * baseThrowSpeed - targetSpeed * targetSpeed;
        float B = (2f * D * targetSpeed * cosTheta);
        float C = -D * D;

        if (B * B < 4f * A * C)
        {
            // it's impossible to hit the target
            return forward * 10f + up * 8f;
        }

        // quadratic equation has two possible outputs
        float t1 = (-B + math.sqrt(B * B - 4f * A * C)) / (2f * A);
        float t2 = (-B - math.sqrt(B * B - 4f * A * C)) / (2f * A);

        // our two t values represent two possible trajectory durations.
        // pick the best one - whichever is lower, as long as it's positive
        float t;
        if (t1 < 0f && t2 < 0f)
        {
            // both potential collisions take place in the past!
            return forward * 10f + up * 8f;
        }

        if (t1 < 0f && t2 > 0f)
        {
            t = t2;
        }
        else if (t1 > 0f && t2 < 0f)
        {
            t = t1;
        }
        else
        {
            t = math.min(t1, t2);
        }

        float3 output = canVelocity - .5f * new float3(0f, -AnimUtils.gravityStrength, 0f) * t +
                        (canPos - startPos) / t;

        if (math.length(output) > baseThrowSpeed * 2f)
        {
            // the required throw is too serious for us to handle
            return forward * 10f + up * 8f;
        }

        return output;
    }

    protected override void OnCreate()
    {
        m_beginSimEcbSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;

        //1.8^2
        float reachDistSq = 3.24f;
        float reachDuration = 1f;
        float windupDuration = 0.7f;
        float throwDuration = 1.2f;

        Entities
            .WithName("ArmTargetJob")
            .WithNone<ArmGrabbedTag>()
            .ForEach((ref ArmGrabTarget grabTarget,
                ref ArmGrabTimer grabT,
                ref ArmLastRockRecord lastRockRecord,
                in ArmAnchorPos anchorPos,
                in ArmBasesUp armUp,
                in ArmReservedRock reservedRock) =>
            {
                float3 rockPos = GetComponent<LocalToWorld>(reservedRock).Position;
                float rockSize = GetComponent<RockRadiusComponentData>(reservedRock);

                lastRockRecord.pos = rockPos;
                lastRockRecord.size = rockSize;
                lastRockRecord.grabbing = true;
                
                float3 delta = rockPos - anchorPos;
                Debug.Assert(math.lengthsq(delta) > 0, "rock and anchor can not be in the same place");
                float3 flatDelta = delta;
                flatDelta.y = 0f;
                flatDelta = math.normalize(flatDelta);

                grabTarget = rockPos + armUp.value * rockSize * .5f -
                             flatDelta * rockSize * .5f;

                grabT += dt / reachDuration;
                grabT = math.clamp(grabT, 0f, 1f);
            }).ScheduleParallel();

        var windupECB = m_beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();
        var throwECB = m_beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithName("ArmFailedLerpBackJob")
            .WithNone<ArmReservedRock>()
            .ForEach((ref ArmGrabTimer grabT) =>
            {
                grabT -= dt / reachDuration;
                grabT = math.clamp(grabT, 0f, 1f);
            }).ScheduleParallel();

        Entities
            .WithName("ArmGrabbedLerpBackJob")
            .WithAny<ArmGrabbedTag>()
            .ForEach((ref ArmGrabTimer grabT) =>
            {
                grabT -= dt / reachDuration;
                grabT = math.clamp(grabT, 0f, 1f);
            }).ScheduleParallel();


        Entities
            .WithName("ArmIKLerpJob")
            .ForEach((ref ArmIKTarget armIKTarget,
                in ArmIdleTarget armIdleTarget,
                in ArmGrabTarget armGrabTarget,
                in ArmGrabTimer grabT) =>
            {
                armIKTarget.value =
                    math.lerp(armIdleTarget, armGrabTarget, grabT); // grabT is already clamped to [0..1]
            }).ScheduleParallel();


        Entities
            .WithName("ArmWindupJob")
            .ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref ArmIKTarget ikTarget,
                ref ArmWindupTimer windupTimer,
                ref ArmWindupTarget windupTarget,
                ref ArmLastThrowRecord throwRecord,
                in ArmReservedCan reservedCan,
                in ArmAnchorPos armPos
            ) =>
            {
                float3 canPos = GetComponent<Translation>(reservedCan).Value;

                // still winding up...

                float windupT = math.clamp(windupTimer, 0, 1) -
                                math.clamp(2f * throwRecord.throwTimer, 0, 1);

                windupT = 3f * windupT * windupT - 2f * windupT * windupT * windupT;
                ikTarget = math.lerp(ikTarget, windupTarget, math.clamp(windupT, 0, 1));
                float3 flatTargetDelta = canPos - armPos;
                flatTargetDelta.y = 0f;
                flatTargetDelta = math.normalize(flatTargetDelta);

                // windup position is "behind us," relative to the target position
                windupTarget = armPos - flatTargetDelta * 2f +
                               new float3(0, 1, 0) * (3f - windupT * 2.5f);

                windupTimer += dt / windupDuration;

                if (windupTimer >= 1f)
                    windupECB.RemoveComponent<ArmWindupTimer>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        Entities
            .WithName("ArmThrowJob")
            .WithNone<ArmWindupTimer>()
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref ArmIKTarget ikTarget,
                ref ArmLastThrowRecord throwRecord,
                ref ArmLastRockRecord lastRockRecord,
                in ArmReservedCan reservedCan,
                in ArmReservedRock reservedRock,
                in ArmWindupTarget windupTarget
            ) =>
            {
                float3 canPos = GetComponent<Translation>(reservedCan).Value;
                float3 canVelocity = GetComponent<Velocity>(reservedCan).Value;

                // done winding up - actual throw, plus resetting to idle
                throwRecord.throwTimer += dt / throwDuration;
                var throwTimeClamp = math.clamp(throwRecord.throwTimer, 0, 1);

                var isRockHeld = HasComponent<Parent>(reservedRock);

                if (isRockHeld)
                {
                    throwRecord.lastAimVelocity = AimAtCan(canPos, lastRockRecord.pos, canVelocity);
                }
                
                // we start this animation in our windup position,
                // and end it by returning to our default idle pose
                
                float3 restingPos = math.lerp(windupTarget, ikTarget, throwTimeClamp);

                // find the hand's target position to perform the throw
                // (somewhere forward and upward from the windup position)
                float3 throwHandTarget = windupTarget + math.normalize(throwRecord.lastAimVelocity) * 2.5f;


                // This lerp is intentionally unclamped, as the throw curve ranges above 1.0
                ikTarget =
                    math.lerp(restingPos, throwHandTarget, AnimUtils.EvauateThrowCurveSmooth(throwTimeClamp));

                

                if (throwRecord.throwTimer > .15f && isRockHeld)
                {
                    // release the rock
                    var rockWorldPos = GetComponent<LocalToWorld>(reservedRock).Position;
                    lastRockRecord.grabbing = false;
                    
                    throwECB.RemoveComponent<Parent>(entityInQueryIndex, reservedRock);
                    throwECB.SetComponent(entityInQueryIndex, reservedRock, new Translation
                    {
                        Value = rockWorldPos
                    });

                    throwECB.RemoveComponent<LocalToParent>(entityInQueryIndex, reservedRock);
                    throwECB.RemoveComponent<RockGrabbedTag>(entityInQueryIndex, reservedRock);
                    
                    throwECB.SetComponent(entityInQueryIndex, reservedRock, new Velocity()
                    {
                        Value = throwRecord.lastAimVelocity
                    });
                    
                    throwECB.AddComponent(entityInQueryIndex, reservedRock, new Acceleration()
                    {
                        Value = new float3(0,-AnimUtils.gravityStrength,0)
                    });
                    
                    throwECB.AddComponent(entityInQueryIndex, reservedRock, new DestroyBoundsY()
                    {
                        Value = -10f
                    });
                    //heldRock = null;
                }

                if (throwRecord.throwTimer >= 1f)
                {
                    throwECB.RemoveComponent<ArmWindupTarget>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmLastThrowRecord>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmReservedRock>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmLastThrowRecord>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmWindupTarget>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmReservedCan>(entityInQueryIndex, entity);
                    throwECB.RemoveComponent<ArmGrabbedTag>(entityInQueryIndex, entity);

                    //heldRock = null;
                }
            }).ScheduleParallel();
            
        m_beginSimEcbSystem.AddJobHandleForProducer(Dependency);


        var grabEcb = m_beginSimEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithName("ArmGrabJob")
            .WithNone<ArmGrabbedTag>()
            .ForEach((
                int entityInQueryIndex,
                Entity armEntity,
                ref ArmReservedRock reservedRock,
                ref ArmLastRockRecord lastRockRecord,
                in ArmGrabTimer grabT,
                in ArmAnchorPos anchorPos,
                in Wrist wrist
            ) =>
            {
                float3 anchorToRock = GetComponent<Translation>(reservedRock).Value - anchorPos;

                if (grabT >= 1.0f)
                {
                    if (!HasComponent<Parent>(reservedRock.value))
                    {
                        Parent wristParent = new Parent
                        {
                            Value = wrist
                        };

                        grabEcb.SetComponent(entityInQueryIndex, reservedRock, new Translation
                        {
                            Value = new float3(0, lastRockRecord.size * -0.1f, lastRockRecord.size * 0.55F)
                        });
                        
                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, wristParent);
                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, new LocalToParent());
                        grabEcb.AddComponent(entityInQueryIndex, reservedRock, new RockGrabbedTag());

                        grabEcb.AddComponent<ArmGrabbedTag>(entityInQueryIndex, armEntity);
                    }
                }
                else if (math.lengthsq(anchorToRock) > reachDistSq)
                {
                    grabEcb.RemoveComponent<RockReservedTag>(entityInQueryIndex, reservedRock);
                    grabEcb.RemoveComponent<ArmReservedRock>(entityInQueryIndex, armEntity);
                }
            }).ScheduleParallel();
        
        
        m_beginSimEcbSystem.AddJobHandleForProducer(Dependency);
        
        var TranslationFromEntity = GetComponentDataFromEntity<Translation>();
        var RotationFromEntity = GetComponentDataFromEntity<Rotation>();

        Entities
            .WithName("armIKJob")
            .WithNativeDisableParallelForRestriction(TranslationFromEntity)
            .WithNativeDisableParallelForRestriction(RotationFromEntity)
            .ForEach((
                int entityInQueryIndex,
                ref ArmBasesForward armForward,
                ref ArmBasesRight armRight,
                ref ArmBasesUp armUp,
                ref DynamicBuffer<ArmJointElementData> armJoints,
                ref Wrist wrist,
                in ArmIKTarget armTarget,
                in ArmAnchorPos armAnchorPos
            ) =>
            {
                FABRIK.Solve(armJoints.AsNativeArray().Reinterpret<float3>(), 1f, armAnchorPos.value, armTarget.value,
                    0.1f * armUp.value);

                //SetComponent(wrist,new Translation
                TranslationFromEntity[wrist] = new Translation
                {
                    Value = armJoints[armJoints.Length - 1].value
                };

                float3 transformRight = new float3(1, 0, 0);
                armForward.value =
                    math.normalize(armJoints[armJoints.Length - 1].value - armJoints[armJoints.Length - 2].value);
                armUp.value = math.normalize(math.cross(armForward.value, transformRight));
                armRight.value = math.cross(armUp.value, armForward.value);

                RotationFromEntity[wrist] = new Rotation
                {
                    Value = quaternion.LookRotation(armForward, armUp)
                };
            }).ScheduleParallel();

        Entities
            .WithName("ArmRockRecordJob")
            .ForEach((ref ArmLastRockRecord lastRockRecord,
                in ArmReservedRock reservedRock) =>
            {
                float3 rockPos = GetComponent<LocalToWorld>(reservedRock).Position;
                float rockSize = GetComponent<RockRadiusComponentData>(reservedRock);

                lastRockRecord.pos = rockPos;
                lastRockRecord.size = rockSize;
            }).ScheduleParallel();
    }
}