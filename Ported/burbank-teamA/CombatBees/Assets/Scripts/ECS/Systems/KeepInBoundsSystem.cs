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
                    velocity.Value.x = -math.sign(t.Value.x)*math.abs(velocity.Value.x);
                    t.Value.x = math.sign(t.Value.x)*(gameBounds.x-1);
                }

                if (math.abs(t.Value.y) > gameBounds.y)
                {
                    velocity.Value.y = -math.sign(t.Value.y) * math.abs(velocity.Value.y);
                    t.Value.y = math.sign(t.Value.y) * (gameBounds.y - 1);

                }

                if (math.abs(t.Value.z) > gameBounds.z )
                {
                    velocity.Value.z = -math.sign(t.Value.z) * math.abs(velocity.Value.z);
                    t.Value.z = math.sign(t.Value.z) * (gameBounds.z - 1);

                }
            })
            .Schedule(inputDeps);
    }
}