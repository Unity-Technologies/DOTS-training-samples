using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class KeepInBoundsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>();

        return Entities.WithNone<ResourceTag>()
            .ForEach((ref Translation t, ref Velocity velocity) =>
            {
                if (math.abs(t.Value.x) > gameBounds.Value.x-gameBounds.threshold)
                {
                    velocity.Value.x *= -1;
                }

                if (math.abs(t.Value.y) > gameBounds.Value.y - gameBounds.threshold)
                {
                    velocity.Value.y *= -1;
                }

                if (math.abs(t.Value.z) > gameBounds.Value.z - gameBounds.threshold)
                {
                    velocity.Value.z *= -1;
                }
            })
            .Schedule(inputDeps);
    }
}