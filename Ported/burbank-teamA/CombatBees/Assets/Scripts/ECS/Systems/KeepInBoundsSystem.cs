using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class KeepInBoundsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>().Value;

        return Entities.WithNone<GravityMultiplier>()
            .ForEach((ref Translation t, ref Velocity velocity) =>
            {
                if (math.abs(t.Value.x) > gameBounds.x)
                {
                    velocity.Value.x *= -1;
                }

                if (math.abs(t.Value.y) > gameBounds.y)
                {
                    velocity.Value.y *= -1;
                }

                if (math.abs(t.Value.z) > gameBounds.z)
                {
                    velocity.Value.z *= -1;
                }
            })
            .Schedule(inputDeps);
    }
}