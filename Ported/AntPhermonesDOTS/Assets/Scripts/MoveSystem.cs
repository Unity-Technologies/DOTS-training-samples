using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = Entities
            .WithName("MoveSystem")
            .WithAll<AntTag>()
            .ForEach((Entity entity, ref Translation translation, ref Rotation rotation, in Velocity velocity) =>
            {
                var y = math.sin(velocity.Rotation);
                var x = math.cos(velocity.Rotation);
                translation.Value += new float3(x, y, 0) * velocity.Speed;
                rotation.Value = Quaternion.AngleAxis(velocity.Rotation / (2*math.PI) * 360.0f, Vector3.forward);
            }).Schedule(inputDeps);
        return jobHandle;
    }
}
