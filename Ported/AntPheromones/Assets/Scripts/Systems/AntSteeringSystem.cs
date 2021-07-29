using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class AntSteeringSystem : SystemBase
{
    const float randomSteering = 0.14f;

    protected override void OnUpdate()
    {
        var rand = new Random(1234);

        Entities
            .WithAll<Ant>()
            .ForEach((ref FacingAngle facingAngle) =>
            {
                facingAngle.Value += rand.NextFloat(-randomSteering, randomSteering);
            }).Schedule();
    }
}
