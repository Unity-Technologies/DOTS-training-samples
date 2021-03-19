using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class AnimationSystem : SystemBase
{
  
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        var rotations = GetComponentDataFromEntity<Rotation>();
        var translations = GetComponentDataFromEntity<Translation>();
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>();
        
        var handsWindingUp = GetComponentDataFromEntity<HandWindingUp>();
        var handsThrowingRock = GetComponentDataFromEntity<HandThrowingRock>();
        var handsIdle = GetComponentDataFromEntity<HandIdle>();
        var handsLookingForACan = GetComponentDataFromEntity<HandLookingForACan>();
        
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        
        var parameters = GetSingleton<SimulationParameters>();
        
        // Initialize hand target position when idle
        Dependency = Entities
            .WithAny<HandIdle, HandLookingForACan>()
            .ForEach((Entity entity, ref TargetPosition targetPosition,
                in Translation translation) =>
            {
                targetPosition = new TargetPosition()
                {
                    Value = translation.Value + new float3(0.0f, 1.0f, 1.0f) * parameters.ArmJointLength * 0.85f
                };
            }).ScheduleParallel(Dependency);
 

        // Initialize hand target position when winding-up
        Dependency = Entities
            .WithAll<HandWindingUp>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration,
                in Translation translation,
                in TargetCan targetCan) =>
            {
                var canPosition = translations[targetCan.Value];
                float3 windUpOffset = translation.Value - canPosition.Value;
                windUpOffset.y = 0.0f;
                windUpOffset = math.normalize(windUpOffset) * parameters.ArmJointLength * 1.75f;
                windUpOffset.y = parameters.ArmJointLength * 0.75f;
                
                targetPosition = new TargetPosition()
                {
                    Value = translations[entity].Value + windUpOffset
                };
            }).ScheduleParallel(Dependency);

        // Initialize hand target position when throwing
        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration,
                in Translation translation,
                in TargetCan targetCan) =>
            {
                var canPosition = translations[targetCan.Value];
                float3 throwOffset = canPosition.Value - translation.Value;
                throwOffset.y *= 10.0f;
                throwOffset = math.normalize(throwOffset) * parameters.ArmJointLength * 1.95f;

                targetPosition = new TargetPosition()
                {
                    Value = translations[entity].Value + throwOffset
                };
            }).ScheduleParallel(Dependency);
        
        
        // Hand moves toward the rock when trying to grab it
        Dependency = Entities
            .WithAll<HandGrabbingRock>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer, in TargetRock targetRock, in Translation translation) =>
            {
                targetPosition.Value = translations[targetRock.RockEntity].Value;

            }).ScheduleParallel(Dependency);

        // Update animation
        Dependency = Entities
            .WithNativeDisableContainerSafetyRestriction(translations)
            .WithNativeDisableContainerSafetyRestriction(rotations)
            .WithReadOnly(localToWorlds)
            .WithReadOnly(handsWindingUp)
            .WithReadOnly(handsThrowingRock)
            .WithReadOnly(handsIdle)
            .WithReadOnly(handsLookingForACan)
            .ForEach((Entity entity,
                ref Arm arm, 
                ref Timer timer, 
                ref AnimStartPosition startPosition, 
                in TargetPosition targetPosition,
                in TimerDuration timerDuration,
                in TargetRock targetRock
                ) =>
            {
                if (Utils.IsPlayingAnimation(timer))
                {
                    var isThrowing = handsThrowingRock.HasComponent(entity);
                    var isWindingUp = handsWindingUp.HasComponent(entity);
                    var isIdle = handsIdle.HasComponent(entity);
                    var isLookingForACan = handsLookingForACan.HasComponent(entity);

                    if (Utils.DidAnimJustStarted(timer, timerDuration))
                    {
                        // when anim start playing, we initialize start position with the current hand position
                        var forearmMatrix = localToWorlds[arm.m_Forearm];
                        startPosition.Value = forearmMatrix.Position + forearmMatrix.Up * 3.0f;
                    }

                    // tick animation timer
                    timer.Value -= deltaTime;

                    var normalizedTime = 1.0f - math.clamp(timer.Value / timerDuration.Value, 0.0f, 1.0f);
                    normalizedTime = Utils.CubicInterpolation(normalizedTime);
                    if (isThrowing)
                    {
                        normalizedTime = Utils.CubicInterpolation(normalizedTime);
                    }
                    var handTargetPos = math.lerp(startPosition.Value, targetPosition.Value, normalizedTime);
                    
                    // Orient arm toward target
                    var armRoot = translations[entity].Value;
                    var forward = math.normalize(handTargetPos - armRoot);
                    var up = new float3(0.0f, forward.z, -forward.y);
                    var rotation  = math.mul(quaternion.LookRotation(forward, up), quaternion.RotateX(math.PI * 0.5f));

                    float limbLength = parameters.ArmJointLength + parameters.ArmJointSpacing;
                    Utils.ExtendIK(limbLength, limbLength, math.distance(handTargetPos, armRoot),
                        out float armAngle, out float forearmAngle);
                    
                    rotations[arm.m_Humerus] = new Rotation()
                    {
                        Value = math.mul(rotation, quaternion.RotateX(-armAngle))
                    };
                    
                    rotations[arm.m_Forearm] = new Rotation()
                    {
                        Value = quaternion.RotateX(-forearmAngle)
                    };
                    
                    if (isWindingUp || isThrowing || isLookingForACan)
                    {
                        // Update rock position to stick to the hand when it's grabbed
                        translations[targetRock.RockEntity] = new Translation()
                        {
                            Value = handTargetPos
                        };
                    }
                }
            }).ScheduleParallel(Dependency);

        // Transition from wind-up -> throw when animation is finished
        Dependency = Entities
            .WithAll<HandWindingUp>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Timer timer,
                ref TimerDuration timerDuration) =>
            {
                if (Utils.DidAnimJustFinished(timer))
                {
                    // Go to Throwing state
                    Utils.GoToState<HandWindingUp, HandThrowingRock>(ecb, entityInQueryIndex, entity, 0.4f);
                }
            }).ScheduleParallel(Dependency);
        
        // Throw rock when throw animation is finished
        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Timer timer,
                ref TimerDuration timerDuration,
                in TargetRock targetRock) =>
            {
                if (Utils.DidAnimJustFinished(timer))
                {
                    // release rock
                    ecb.RemoveComponent<Grabbed>(entityInQueryIndex, targetRock.RockEntity);
                    
                    // reset target
                    ecb.SetComponent(entityInQueryIndex, entity, new TargetRock());
                    ecb.SetComponent(entityInQueryIndex, entity, new TargetCan());

                    // Go to Idle state
                    Utils.GoToState<HandThrowingRock, HandIdle>(ecb, entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        sys.AddJobHandleForProducer(Dependency);
    }
}
