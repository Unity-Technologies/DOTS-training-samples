using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = Entities
            .WithName("SpawnerSystem")
            .WithAll<AntTag>()
            .ForEach((Entity entity, ref Translation translation) =>
            {
                translation.Value.x += 0.01f;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
