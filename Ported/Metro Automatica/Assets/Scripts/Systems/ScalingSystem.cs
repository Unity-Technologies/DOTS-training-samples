using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial class ScalingSystem : SystemBase
{
    protected override void OnCreate()
    {
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Station>()
            .WithAll<LocalTransform>()
            .Build(this);

        var variable = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        //Debug.Log(variable.Length);
        foreach (var localTransform in variable)
        {
            //Debug.Log(localTransform.Position);
        }
        //Debug.Log(variable);
    }
    protected override void OnUpdate()
    {
        // The Entities.ForEach below is Burst compiled (implicitly).
        // And time is a member of SystemBase, which is a managed type (class).
        // This means that it wouldn't be possible to directly access Time from there.
        // So we need to copy the value we need (DeltaTime) into a local variable.
        var dt = SystemAPI.Time.DeltaTime;

        // Entities.ForEach is an older approach to processing queries. Its use is not
        // encouraged, but it remains convenient until we get feature parity with IFE.
        /*Entities
            .WithAll<Station>()
            .ForEach((Entity entity, TransformAspect transform) =>
            {
                //Debug.Log("entity.Index");
                // Notice that this is a lambda being passed as parameter to ForEach.
                /*var pos = transform.Position;

                // This does not modify the actual position of the tank, only the point at
                // which we sample the 3D noise function. This way, every tank is using a
                // different slice and will move along its own different random flow field.
                pos.y = entity.Index;

                // Unity.Mathematics.noise provides several types of noise functions.
                // Here we use the Classic Perlin Noise (cnoise).
                // The approach taken to generate a flow field from Perlin noise is detailed here:
                // https://www.bit-101.com/blog/2021/07/mapping-perlin-noise-to-angles/
                var angle = (0.5f + noise.cnoise(pos / 10f)) * 4.0f * math.PI;

                var dir = float3.zero;
                math.sincos(angle, out dir.x, out dir.z);
                transform.Position += dir * dt * 5.0f;
                transform.Rotation = quaternion.RotateY(angle);

                // The last function call in the Entities.ForEach sequence controls how the code
                // should be executed: Run (main thread), Schedule (single thread, async), or
                // ScheduleParallel (multiple threads, async).
                // Entities.ForEach is fundamentally a job generator, and it makes it very easy to
                // create parallel jobs. This unfortunately comes with a complexity cost and weird
                // arbitrary constraints, which is why more explicit approaches are preferred.
                // Those explicit approaches (IJobEntity) are covered later in this tutorial.
            }).ScheduleParallel();*/
    }
}