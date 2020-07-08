using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(VelocitySystem))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float delta = UnityEngine.Time.deltaTime;
        
        Entities
            .ForEach((Entity entity, ref Translation translation, in Velocity velocity) =>
            {
                translation.Value += velocity.Value * delta;
            }).ScheduleParallel();
    }
}
