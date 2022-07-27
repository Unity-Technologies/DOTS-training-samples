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
            var et = (float)Time.ElapsedTime;
            var speed = 10f;
            float offsetValue = 1f;
            Random random = Random.CreateFromIndex((uint)System.DateTime.Now.Millisecond);
            float3 target = new float3(5, 0, 0);
            float3 yellowBase = new float3(-45, 0, 0);
            float3 blueBase = new float3(45, 0, 0);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
        
            Entities
            .ForEach((ref Translation translation, ref Bee bee, ref Entity beeEntity,  ref Rotation rotation) =>
            {
                var position = translation.Value;
                
                SpeedHandler(bee);

                var offset = new float3(
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue))
                );

                if (HasComponent<LocalToWorld>(bee.target))
                {
                    target = GetComponent<LocalToWorld>(bee.target).Position;
                }
                else
                {
                    bee.state = BeeState.Idle;
                    target = HasComponent<YellowTeam>(beeEntity) ? yellowBase : blueBase;
                }
                
                var direction = math.normalizesafe( target - translation.Value);
                
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
                position +=  (direction + offset) * speed * dt;
                CheckBounds(ref position);
                
                translation.Value = position;
                
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
                        var component = new Food();
                        if (HasComponent<Food>(bee.target))
                        {
                            component = GetComponent<Food>(bee.target);
                        }
                        
                        component.target = beeEntity;
                        component.targetPos = translation.Value;
                        target = HasComponent<YellowTeam>(beeEntity) ? yellowBase : blueBase;
                        bee.target = new Entity();
                        bee.state = BeeState.Hauling;
                    }
                }

                if (bee.state == BeeState.Hauling)
                {
                    if (math.distancesq(position, target) < 0.5f)
                    {
                        bee.state = BeeState.Idle;
                    }
                }
                
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

            void SpeedHandler(Bee bee)
            {
                if (bee.state == BeeState.Attacking) speed = 50f;
                if (bee.state == BeeState.Collecting) speed = 25f;
                if (bee.state == BeeState.Hauling) speed = 20f;
            }

            
    }

    
    
}
