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
        
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var ecb = sys.CreateCommandBuffer().AsParallelWriter();

        // Initialize hand target position when winding-up
        Dependency = Entities
            .WithAll<HandWindingUp>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration) =>
            {
                if (Utils.DidAnimJustStarted(timer, timerDuration))
                {
                    targetPosition = new TargetPosition()
                    {
                        Value = translations[entity].Value + new float3(
                            0.0f,
                            3.0f,
                            -6.0f)
                    };
                }
            }).ScheduleParallel(Dependency);

        // Initialize hand target position when throwing
        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration) =>
            {
                if (Utils.DidAnimJustStarted(timer, timerDuration))
                {
                    targetPosition = new TargetPosition()
                    {
                        Value = translations[entity].Value + new float3(
                            0.0f,
                            6.0f,
                            6.0f)
                    };
                }
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
            .WithNone<HandIdle>()
            .WithNativeDisableContainerSafetyRestriction(translations)
            .WithNativeDisableContainerSafetyRestriction(rotations)
            .WithReadOnly(localToWorlds)
            .WithReadOnly(handsWindingUp)
            .WithReadOnly(handsThrowingRock)
            .ForEach((Entity entity, ref Arm arm, 
                ref Timer timer, 
                ref AnimStartPosition startPosition, 
                ref TargetPosition targetPosition,
                in TimerDuration timerDuration,
                in TargetRock targetRock) =>
            {
                if (Utils.IsPlayingAnimation(timer))
                {
                    var isThrowing = handsThrowingRock.HasComponent(entity);
                    var isWindingUp = handsWindingUp.HasComponent(entity);
                    
                    if (Utils.DidAnimJustStarted(timer, timerDuration))
                    {
                        // when anim start playing, we initialize start position with the current hand position
                        var forearmMatrix = localToWorlds[arm.m_Forearm];
                        startPosition.Value = forearmMatrix.Position + forearmMatrix.Up * 3.0f;
                    }

                    // tick animation timer
                    timer.Value -= deltaTime;

                    var normalizedTime = 1.0f - math.clamp(timer.Value / timerDuration.Value, 0.0f, 1.0f);
                    var handTargetPos = math.lerp(startPosition.Value, targetPosition.Value, normalizedTime);
                    
                    // Orient arm toward target
                    var armRoot = translations[entity].Value;
                    var forward = math.normalize(handTargetPos - armRoot);
                    var moveDirection = targetPosition.Value - startPosition.Value;
                    if (isWindingUp)
                    {
                        moveDirection = -moveDirection;
                    }

                    var right = math.cross(moveDirection, forward);
                    right.y = right.y * 0.25f; // Keep hand quite horizontal
                                     
                    var currentUp = -localToWorlds[arm.m_Humerus].Forward;
                    var up = math.cross(right, forward);
                    up = math.lerp(currentUp, up, deltaTime);

                    var rotation  = math.mul(quaternion.LookRotation(forward, up), quaternion.RotateX(math.PI * 0.5f));
                    
                    rotations[arm.m_Humerus] = new Rotation()
                    {
                        Value = rotation
                    };
                    
                    if (isWindingUp || isThrowing)
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
                    Utils.GoToState<HandWindingUp, HandThrowingRock>(ecb, entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        // Throw rock when throw animation is finished
        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Timer timer,
                ref TimerDuration timerDuration) =>
            {
                if (Utils.DidAnimJustFinished(timer))
                {
                    // reset target
                    ecb.SetComponent(entityInQueryIndex, entity, new TargetRock());
                    
                    // Go to Idle state
                    Utils.GoToState<HandThrowingRock, HandIdle>(ecb, entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        sys.AddJobHandleForProducer(Dependency);
    }
}
