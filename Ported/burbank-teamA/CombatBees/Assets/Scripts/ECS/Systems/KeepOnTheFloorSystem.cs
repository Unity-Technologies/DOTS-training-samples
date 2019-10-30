using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class KeepOnTheFloorSystem : JobComponentSystem
{
   protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>();

        return Entities.WithAll<ResourceTag>()
            .ForEach((ref Translation t, ref Velocity velocity) =>
            {
                if(t.Value.y<-gameBounds.Value.y*gameBounds.threshold)
                    velocity.Value = new float3();
            })
            .Schedule(inputDeps);
    }
}
