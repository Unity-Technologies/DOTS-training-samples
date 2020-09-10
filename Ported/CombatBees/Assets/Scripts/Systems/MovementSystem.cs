using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// initial resources falling
// death
// resources falling

public class GravitySystem : SystemBase
{

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var b = GetSingleton<BattleField>();

        Entities.WithNone<Parent>().ForEach((ref Translation translation, ref Velocity velocity) =>
                {
                    velocity.Value.y -= deltaTime * 9.8f;
                    translation.Value += velocity.Value * deltaTime;
                    if (translation.Value.y < -b.Bounds.y / 2)
                    {
                        velocity.Value.y *= -1;
                        velocity.Value *= 0.3f;
                    }
                    translation.Value.y = math.clamp(translation.Value.y, -b.Bounds.y / 2, b.Bounds.y / 2);

                }).Schedule();
     }


}
