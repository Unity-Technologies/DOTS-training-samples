using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class KeepInBoundsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>().Value;

        return Entities.WithAll<BeeTag>()
            .ForEach((ref Translation t, ref Velocity velocity, ref TargetVelocity tv) =>
            {
                if (math.abs(t.Value.x) > gameBounds.x)
                {
                    velocity.Value.x *= -1;
                    t.Value.x -= math.sign(t.Value.x)*0.5f;
                }

                if (math.abs(t.Value.y) > gameBounds.y)
                {
                    velocity.Value.y *= -1;
                    t.Value.y -= math.sign(t.Value.y) * 0.5f;

                }

                if (math.abs(t.Value.z) > gameBounds.z )
                {
                    velocity.Value.z *= -1;
                    t.Value.z -= math.sign(t.Value.z) * 0.5f;

                }
            })
            .Schedule(inputDeps);
    }
}