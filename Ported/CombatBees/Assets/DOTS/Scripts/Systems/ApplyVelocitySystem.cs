using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(GravitySystem))]
public class ApplyVelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("ApplyVelocity")
            .ForEach((ref Translation translation, in Velocity velocity) =>
            {
                translation.Value += velocity.Value * deltaTime;

                // TODO: Arena Bounds Check
                //translation.Value.y = math.max(translation.Value.y, 0.05f);
            }).ScheduleParallel();
    }
}
