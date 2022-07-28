using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial class BeeMovementSystem : SystemBase
{
    ComponentDataFromEntity<NotCollected> _notCollected;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        _notCollected = GetComponentDataFromEntity<NotCollected>();
    }
    
    protected override void OnUpdate()
    {
            _notCollected.Update(this);

            var notCollected = _notCollected;
            var dt = Time.DeltaTime;
            var et = (float)Time.ElapsedTime;
            var speed = 10f;
            var offsetValue = 1f;
            var random = Random.CreateFromIndex(GlobalSystemVersion);
            var target = new float3(5, 0, 0);

            var baseComponent = GetSingleton<Base>();
            var baseEntity = GetSingletonEntity<Base>();
            var entityManager = new EntityManager();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
        
            Entities
            .ForEach((ref Translation translation, ref Bee bee, ref Entity beeEntity,  ref Rotation rotation) =>
            {
                var randomPlaceInSpawn = HasComponent<YellowTeam>(beeEntity) ?
                    random.NextFloat3(baseComponent.yellowBase.GetBaseLowerLeftCorner(), baseComponent.yellowBase.GetBaseUpperRightCorner()) 
                    : random.NextFloat3(baseComponent.blueBase.GetBaseLowerLeftCorner(), baseComponent.blueBase.GetBaseUpperRightCorner());
                
                var position = translation.Value;
                
                SpeedHandler(bee);
                var newScale = new LocalToWorld{
                    Value = float4x4.TRS(
                        translation:    position ,
                        rotation:        rotation.Value ,
                        scale:            new float3(speed, speed, speed)
                    )
                };
                SetComponent(beeEntity, newScale);

                var offset = new float3(
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue))
                );

                if (bee.target == baseEntity)
                {
                    target = bee.targetPos;
                }
                else if (bee.target != Entity.Null && Exists(bee.target))
                {
                    target = GetComponent<LocalToWorld>(bee.target).Position;
                }
                else
                {
                    bee.state = BeeState.Idle;
                    bee.target = baseEntity;
                    bee.targetPos = randomPlaceInSpawn;
                    
                    target = bee.targetPos;
                }
                
                var direction = math.normalizesafe( target - translation.Value);
                
                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0,1,0));
                position +=  (direction + offset) * speed * dt;
                CheckBounds(ref position);
                
                translation.Value = position;
                
                if (bee.state == BeeState.Attacking)
                {
                    if (math.distancesq(position, target) < 0.25f)
                    {
                        bee.targetPos = position;
                        ecb.DestroyEntity(bee.target);
                        
                        bee.target = Entity.Null;
                        bee.state = BeeState.Idle;
                    }
                }
                
                if (bee.state == BeeState.Collecting)
                {
                    if (math.distancesq(position, target) < 0.25f)
                    {
                        var component = GetComponent<Food>(bee.target);
                        component.target = beeEntity;
                        
                        notCollected.SetComponentEnabled(bee.target, false);

                        SetComponent(bee.target, component);

                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        bee.state = BeeState.Hauling;
                        
                        target = bee.targetPos;

                    }
                }

                if (bee.state == BeeState.Hauling)
                {
                    if (math.distancesq(position, target) < 0.25f)
                    {
                        bee.state = BeeState.Idle;
                        
                        bee.target = Entity.Null;
                    }
                }
                
            }).Run();
            ecb.Playback(EntityManager);
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
                if (bee.state == BeeState.Attacking) speed = random.NextFloat(30f, 70f);
                if (bee.state == BeeState.Collecting) speed = random.NextFloat(15f, 30f);
                if (bee.state == BeeState.Hauling) speed = random.NextFloat(10f, 30f);;
            }

            
    }

    
    
}
