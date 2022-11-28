using Unity.Entities;
using UnityEngine;

public partial class MovementSystem : SystemBase
{
   // protected override void OnUpdate()
   // {
   //     // Local variable captured in ForEach
   //     float dT = Time.DeltaTime;
   //
   //     Entities
   //         .WithName("Update_Displacement")
   //         .ForEach(
   //             (ref Position position, in Velocity velocity) =>
   //             {
   //                 position = new Position()
   //                 {
   //                     Value = position.Value + velocity.Value * dT
   //                 };
   //             }
   //         )
   //         .ScheduleParallel();
   // }
   protected override void OnUpdate()
   {
      throw new System.NotImplementedException();
   }
}
