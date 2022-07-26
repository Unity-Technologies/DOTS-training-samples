using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Contrarily to ISystem, SystemBase systems are classes.
// They are not Burst compiled, and can use managed code.
partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // The Entities.ForEach below is Burst compiled (implicitly).
        // And time is a member of SystemBase, which is a managed type (class).
        // This means that it wouldn't be possible to directly access Time from there.
        // So we need to copy the value we need (DeltaTime) into a local variable.
        var dt = Time.DeltaTime;
        var frameCount = UnityEngine.Time.frameCount;
        Random rand = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
        float moveSpeed = 8f;
        float ocilateMag = 20f;

        // Entities.ForEach is an older approach to processing queries. Its use is not
        // encouraged, but it remains convenient until we get feature parity with IFE.
        Entities
            .WithAny<BlueBee, YellowBee>()
            .ForEach((Entity entity, TransformAspect transform, ref Bee beeData) =>
            {
                var pos = transform.Position;
                var dir = beeData.Target - pos;
                float mag = (dir.x * dir.x) + (dir.y * dir.y) + (dir.z * dir.z);
                float dist = math.sqrt(mag);

                dir += beeData.OcillateOffset;

                if (dist > 0)
                    transform.Position += (dir / dist) * dt * moveSpeed;
                
                if (dist <= 2f)
                {
                    if (beeData.Target.x == 0 &&
                        beeData.Target.y == 0 &&
                        beeData.Target.z == 0)
                    {
                        beeData.Target = rand.NextFloat3(-50, 50);
                    }
                    else
                    {
                        beeData.Target = float3.zero;
                    }
                }

                if (frameCount % 30 == 0)
                    beeData.OcillateOffset = rand.NextFloat3(-ocilateMag, ocilateMag);

                // The last function call in the Entities.ForEach sequence controls how the code
                // should be executed: Run (main thread), Schedule (single thread, async), or
                // ScheduleParallel (multiple threads, async).
                // Entities.ForEach is fundamentally a job generator, and it makes it very easy to
                // create parallel jobs. This unfortunately comes with a complexity cost and weird
                // arbitrary constraints, which is why more explicit approaches are preferred.
                // Those explicit approaches (IJobEntity) are covered later in this tutorial.
            }).ScheduleParallel();
    }
}