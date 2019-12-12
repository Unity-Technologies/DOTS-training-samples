using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = Entities
            .WithName("MoveSystem")
            .WithAll<AntTag>()
            .ForEach((Entity entity, ref Translation translation, in Velocity velocity) =>
            {
                var y = math.sin(velocity.Rotation);
                var x = math.cos(velocity.Rotation);
                translation.Value += new float3(y, x, 0) * velocity.Speed;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
