using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
            var dt = Time.DeltaTime;
            var speed = 5f;
            float offsetValue = 1f;
            Random random = new Random(12344);
            float3 target = new float3(5, 0, 0);
            float3 yellowBase = new float3(-45, 0, 0);
            float3 blueBase = new float3(45, 0, 0);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
        
            Entities
            .ForEach((ref Translation translation, ref Bee bee, ref Entity beeEntity,  ref Rotation rotation) =>
            {
                var position = translation.Value;
                var offset = new float3(
                    random.NextFloat(-1 * offsetValue, offsetValue),
                    random.NextFloat(-1 * offsetValue, offsetValue),
                    random.NextFloat(-1 * offsetValue, offsetValue)
                );

                if (HasComponent<LocalToWorld>(bee.target))
                {
                    target = GetComponent<LocalToWorld>(bee.target).Position;
                }
                
                var direction = math.normalizesafe( target - translation.Value);
                
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
                position +=  (offset +direction) * speed * dt;
                CheckBounds(ref position);
                
                if (bee.state == BeeState.Attacking)
                {
                    if (math.distancesq(position, target) < 0.5f)
                    {
                        bee.targetPos = position;
                        ecb.DestroyEntity(bee.target);
                        bee.state = BeeState.Idle;
                    }
                }
                
                if (bee.state == BeeState.Collecting)
                {
                    if (math.distancesq(position, target) < 0.5f)
                    {
                        bee.targetPos = position;
                        bee.state = BeeState.Hauling;

                        var component = GetComponent<Food>(bee.target);
                        component.target = beeEntity;
                    }
                }
                

                if (HasComponent<YellowTeam>(beeEntity))
                {
                    
                }
                
                translation.Value = position;

            }).Run();
            ecb.Playback(this.EntityManager);
            ecb.Dispose();
            
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
