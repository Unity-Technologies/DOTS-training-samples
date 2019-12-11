using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

struct Velocity: IComponentData
{
    public float Rotation;
    public float Speed;
}

public class SteeringSystem: JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = Entities
            .WithName("SpawnerSystem")
            .WithAll<AntTag>()
            .ForEach((Entity entity, ref Velocity velocity) =>
            {
                velocity.Rotation += 0.01f;
                velocity.Speed = 0.1f;
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
