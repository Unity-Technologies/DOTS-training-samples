using DOTSRATS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSRATS
{
    public class RotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities
                .WithAll<InPlay>()
                .ForEach((ref Rotation rotation, in Velocity velocity) =>
                {
                    quaternion lookRotation = quaternion.LookRotationSafe(DirectionExt.ToFloat3(velocity.Direction), new float3(0f, 1f, 0f));
                    if (!rotation.Value.Equals(lookRotation))
                    {
                        rotation.Value = Quaternion.Slerp((Quaternion)rotation.Value, (Quaternion)lookRotation, deltaTime*5);
                    }

                }).ScheduleParallel();
        }
    }
}
