using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class JitterSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var dt = Time.deltaTime;
        var gravity = GetSingleton<JitterForce>().Value;
        return Entities
            .ForEach((ref Velocity velocity, ref Translation translation, in Team team) =>
            {
                velocity.Value = math.lerp(velocity.Value, velocity.Value -= math.up() * gravity * dt, 0.5f);
            })
            .Schedule(inputDependencies);
    }
}