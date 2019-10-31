using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class KeepOnTheFloorSystem : JobComponentSystem
{
   protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>().Value;

        return Entities.WithAny<ResourceTag, ParticleTag>()
            .ForEach((ref Translation t, ref Velocity velocity, ref GravityMultiplier gm) =>
            {

                if (t.Value.y < -gameBounds.y)
                {
                    gm.Value = 0;
                    velocity.Value = new float3();
                }

                if (math.abs(t.Value.x) > gameBounds.x)
                {
                    velocity.Value.x = 0;
                }

                if (math.abs(t.Value.z) > gameBounds.z)
                {
                    velocity.Value.z = 0;
                }

            })
            .Schedule(inputDeps);
    }
}
