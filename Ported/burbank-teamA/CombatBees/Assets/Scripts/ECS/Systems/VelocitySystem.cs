using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class VelocitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dt = Time.deltaTime;
        
        return Entities
            .ForEach((ref Translation translation, in Velocity velocity) => { translation.Value += velocity.Value * dt; })
            .Schedule(inputDependencies);
    }
}