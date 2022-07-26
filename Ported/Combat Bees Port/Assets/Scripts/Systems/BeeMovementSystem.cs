using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
            var dt = Time.DeltaTime;
            var speed = 2.5f;
            Random random = new Random(1234);
            float3 target;
                
                target = new float3(5, 0, 0);
        
            Entities
            .ForEach((ref Translation translation, ref Bee bee,  ref Rotation rotation) =>
            {
                var position = translation.Value;
                var offset = new float3(
                    random.NextFloat(-0.5f,0.5f),
                    random.NextFloat(-0.5f,0.5f),
                    random.NextFloat(-0.5f,0.5f)
                );
                

                if (HasComponent<LocalToWorld>(bee.target))
                {
                    target = GetComponent<LocalToWorld>(bee.target).Position;
                }
                
               // var dataComponent = GetComponentDataFromEntity<LocalToWorld>(true);
              //  var targetEntity = bee.target;
               // target = dataComponent[targetEntity].Position;

             //  target = bee.targetPos;
                
                var direction = math.normalizesafe( target - translation.Value);
                
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
                position +=  (offset +direction) * speed * dt;
                CheckBounds(ref position);

                
                translation.Value = position;

            }).ScheduleParallel();
            
            void CheckBounds(ref float3 position)
            {
                if (position.x < -50) position.x = -50;
                if (position.x > 50) position.x = 50;
                if (position.y < -10) position.y = -10;
                if (position.y > 10) position.y = 10;
                if (position.z < -10) position.z = -10;
                if (position.z > 10) position.z = 10;
            }
    }

    
    
}
