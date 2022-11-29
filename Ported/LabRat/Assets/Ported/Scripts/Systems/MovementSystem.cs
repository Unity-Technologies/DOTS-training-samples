using System.Runtime.CompilerServices;
using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct MovementSystem : ISystem
{
   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
   }

   [BurstCompile]
   public void OnDestroy(ref SystemState state)
   {
   }

   // [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
       int numUpdatedEntities = 0;

       foreach (var (unit, pos, transform, entity) in SystemAPI.Query<RefRW<UnitMovementComponent>, RefRW<PositionComponent>, RefRW<WorldTransform>>()
                    .WithEntityAccess())
       {
           numUpdatedEntities++;
           var direction = unit.ValueRO.direction;
           var speed = unit.ValueRO.speed;

           RatLabHelper.DirectionToVector(out var dir, direction);

           pos.ValueRW.position += (dir * speed * SystemAPI.Time.DeltaTime);
           // pos.ValueRW.position.x += 0.01f;

           // todo : check if possible to continue moving?
           // if(!CanMoveInDirection(...)) rotate(...)
           if (pos.ValueRO.position.x > 10)
           {
               unit.ValueRW.direction = MovementDirection.West;
           }
           else if (pos.ValueRO.position.x < -10)
           {
               unit.ValueRW.direction = MovementDirection.East;
           }

           transform.ValueRW.Position.x = pos.ValueRW.position.x;
           transform.ValueRW.Position.y = pos.ValueRW.position.y;
       }

       //Debug.Log($"[MovementSystem] updated entities={numUpdatedEntities}");
   }
}