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
        
        // var handsGrabbingRock = GetComponentDataFromEntity<HandGrabbingRock>();
        var handsWindingUp = GetComponentDataFromEntity<HandWindingUp>();
        var handsThrowingRock = GetComponentDataFromEntity<HandThrowingRock>();
        
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var ecb = sys.CreateCommandBuffer().AsParallelWriter();

        Dependency = Entities
            .WithAll<HandWindingUp>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration) =>
            {
                if (timer.Value >= timerDuration.Value)
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

        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer,
                in TimerDuration timerDuration) =>
            {
                if (timer.Value >= timerDuration.Value)
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
        
        Dependency = Entities
            .WithAll<HandGrabbingRock>()
            .WithReadOnly(translations)
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Timer timer, in TargetRock targetRock) =>
            {
                targetPosition.Value = translations[targetRock.RockEntity].Value;
            }).ScheduleParallel(Dependency);

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
                if (timer.Value > 0.0f)
                {
                    // var isThrowing = handsThrowingRock.HasComponent(entity);
                    
                    if (timer.Value >= timerDuration.Value)
                    {
                        var forearmMatrix = localToWorlds[arm.m_Forearm];
                        startPosition.Value = forearmMatrix.Position + forearmMatrix.Up * 3.0f;
                        
                    }

                    timer.Value -= deltaTime;
                    
                    // if (handsGrabbingRock.HasComponent(entity))
                    // {
                    //     targetPosition.Value = translations[targetRock.RockEntity].Value;
                    // }

                    var progression = 1.0f - math.clamp(timer.Value / timerDuration.Value, 0.0f, 1.0f);
                    var handTargetPos = math.lerp(startPosition.Value, targetPosition.Value, progression);

                    var pos = translations[entity].Value;
                    var targetDir = math.normalize(handTargetPos - pos);

                    rotations[arm.m_Humerus] = new Rotation()
                    {
                        Value = math.mul(quaternion.LookRotation(targetDir, new float3(0.0f, 1.0f, 0.0f)),
                            quaternion.RotateX(math.PI * 0.5f))
                    };
                    
                    if (handsThrowingRock.HasComponent(entity) ||
                        handsWindingUp.HasComponent(entity))
                    {
                        translations[targetRock.RockEntity] = new Translation()
                        {
                            Value = handTargetPos
                        };
                    }
                }
            }).ScheduleParallel(Dependency);

        Dependency = Entities
            .WithAll<HandWindingUp>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Timer timer,
                ref TimerDuration timerDuration) =>
            {
                if (timer.Value <= 0.0f)
                {
                    // Go to Throwing state
                    
                    timer = new Timer() {Value = 1.0f};
                    timerDuration = new TimerDuration() {Value = 1.0f};

                    ecb.RemoveComponent<HandWindingUp>(entityInQueryIndex, entity);
                    ecb.AddComponent<HandThrowingRock>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        Dependency = Entities
            .WithAll<HandThrowingRock>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Timer timer,
                ref TimerDuration timerDuration,
                ref TargetRock targetRock) =>
            {
                if (timer.Value <= 0.0f)
                {
                    ecb.SetComponent(entityInQueryIndex, targetRock.RockEntity, new Velocity()
                    {
                        Value = new float3(0.0f, 12.0f, 240.0f)
                    });
                    ecb.AddComponent<Falling>(entityInQueryIndex, targetRock.RockEntity);
                    
                    // reset target
                    targetRock.RockEntity = new Entity();

                    // Go to Idle state
                    ecb.RemoveComponent<HandThrowingRock>(entityInQueryIndex, entity);
                    ecb.AddComponent<HandIdle>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        sys.AddJobHandleForProducer(Dependency);
    }
}
