using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;

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
      foreach (var (unit, pos, entity) in SystemAPI.Query<RefRO<UnitMovementComponent>, RefRW<PositionComponent>>()
                  .WithEntityAccess())
      {
         var direction = unit.ValueRO.direction;
         var speed = unit.ValueRO.speed;

         var dir = RatLabHelper.DirectionToVector(direction);

         pos.ValueRW.position += (dir * speed * SystemAPI.Time.DeltaTime);
         
         // todo : check if possible to continue moving?
         // if(!CanMoveInDirection(...)) rotate(...)
      }
   }
}
