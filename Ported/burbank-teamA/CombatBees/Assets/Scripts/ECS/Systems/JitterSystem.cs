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
        return Entities.WithAll<BeeTag>().ForEach((ref Translation t, in Velocity v) =>
            {
                //velocity.Value = math.lerp(velocity.Value, velocity.Value -= noise.cnoise(velocity.Value) * gravity * dt, 0.5f);
                t.Value += noise.cnoise(v.Value) * gravity * dt;
            })
            .Schedule(inputDependencies);
    }
}