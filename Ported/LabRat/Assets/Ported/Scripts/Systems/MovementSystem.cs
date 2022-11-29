using System.Runtime.CompilerServices;
using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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

   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
       int numUpdatedEntities = 0;

       foreach (var (unit, pos, entity) in SystemAPI.Query<RefRO<UnitMovementComponent>, RefRW<PositionComponent>>()
                    .WithEntityAccess())
       {
           numUpdatedEntities++;
           var direction = unit.ValueRO.direction;
           var speed = unit.ValueRO.speed;

           RatLabHelper.DirectionToVector(out var dir, direction);

           pos.ValueRW.position += (dir * speed * SystemAPI.Time.DeltaTime);

           // todo : check if possible to continue moving?
           // if(!CanMoveInDirection(...)) rotate(...)
       }

       //Debug.Log($"[MovementSystem] updated entities={numUpdatedEntities}");
   }
}