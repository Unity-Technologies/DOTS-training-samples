using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class MovementSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var b = GetSingleton<BattleField>();

        Entities.WithNone<Parent>().ForEach((ref Translation translation, ref Velocity velocity, ref NonUniformScale nonUniformScale, ref Rotation rotation) =>
        {
            velocity.Value.y -= deltaTime * 9.8f;
            translation.Value += velocity.Value * deltaTime;
            if (translation.Value.y < -b.Bounds.y / 2)
            {
                velocity.Value.y *= -1;
                velocity.Value *= 0.3f;
            }
            translation.Value.y = math.clamp(translation.Value.y, -b.Bounds.y / 2, b.Bounds.y / 2);

            rotation.Value = quaternion.LookRotationSafe(velocity.Value, new float3(0, 1, 0));
            nonUniformScale.Value.z = math.length(velocity.Value) * 0.1f;

        }).ScheduleParallel();

        Entities.WithNone<Parent>().WithNone<NonUniformScale>().ForEach((ref Translation translation, ref Velocity velocity) =>
        {
            velocity.Value.y -= deltaTime * 9.8f;
            translation.Value += velocity.Value * deltaTime;
            if (translation.Value.y < -b.Bounds.y / 2)
            {
                velocity.Value.y *= -1;
                velocity.Value *= 0.3f;
            }
            translation.Value.y = math.clamp(translation.Value.y, -b.Bounds.y / 2, b.Bounds.y / 2);


        }).ScheduleParallel();
    }


}
