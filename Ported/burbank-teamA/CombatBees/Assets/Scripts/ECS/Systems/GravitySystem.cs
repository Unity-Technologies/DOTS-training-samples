using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class GravitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dt = Time.deltaTime;
        var gravity = GetSingleton<Gravity>().Value;
        
        return Entities
            .ForEach((ref Velocity velocity, in GravityMultiplier gravityMultiplier) =>
            {
                velocity.Value -= math.up() * gravity * gravityMultiplier.Value * dt;
            })
            .Schedule(inputDependencies);
    }
}