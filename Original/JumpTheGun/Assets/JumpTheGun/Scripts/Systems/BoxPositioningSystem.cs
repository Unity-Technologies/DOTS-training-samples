using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial class BoxPositioningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Random random;

        random = Random.CreateFromIndex(1234);

        var deltaTime = Time.DeltaTime;

        Entities
            .WithAll<Boxes>()
            .ForEach((Entity entity, TransformAspect transform) =>
            {

                // Notice that this is a lambda being passed as parameter to ForEach.
                var pos = transform.Position;
                pos.x = random.NextFloat(0, 10);
                pos.z = random.NextFloat(0, 10);

                transform.Position = pos;

            }).ScheduleParallel();
    }
}
