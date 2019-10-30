using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class KeepOnTheFloorSystem : JobComponentSystem
{
   protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>().Value;

        return Entities
            .ForEach((ref Velocity velocity) =>
            {
                velocity.Value.y = 0;
            })
            .Schedule(inputDeps);
    }
}
