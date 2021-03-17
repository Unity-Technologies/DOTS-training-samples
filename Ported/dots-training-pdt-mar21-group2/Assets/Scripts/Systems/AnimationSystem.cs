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
        
        Entities
            .WithNone<HandIdle>()
            .ForEach((Entity entity, ref Arm arm, 
                ref Timer timer, 
                ref AnimStartPosition startPosition, 
                in TimerDuration timerDuration,
                in TargetPosition targetPosition) =>
            {
                if (timer.Value > 0.0f)
                {
                    var humerusMatrix = localToWorlds[arm.m_Humerus];
                    if (timer.Value >= timerDuration.Value)
                    {
                        startPosition.Value = humerusMatrix.Position + humerusMatrix.Up * 3.0f;
                    }

                    timer.Value -= deltaTime;

                    var progression = 1.0f - math.clamp(timer.Value / timerDuration.Value, 0.0f, 1.0f);
                    var handTargetPos = math.lerp(startPosition.Value, targetPosition.Value, progression);

                    var pos = translations[entity].Value;
                    var targetDir = math.normalize(handTargetPos - pos);

                    rotations[arm.m_Humerus] = new Rotation()
                    {
                        Value = math.mul(quaternion.LookRotation(targetDir, new float3(0.0f, 1.0f, 0.0f)),
                            quaternion.RotateX(math.PI * 0.5f))
                    };
                    
                }
            }).Run();
        
    }
}
