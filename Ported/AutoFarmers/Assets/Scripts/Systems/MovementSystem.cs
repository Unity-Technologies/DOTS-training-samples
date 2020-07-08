using Unity.Entities;

[UpdateAfter(typeof(VelocitySystem))]
public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float delta = UnityEngine.Time.deltaTime;
        
        Entities
            .ForEach((Entity entity, ref Position position, in Velocity velocity) =>
            {
                position.Value += velocity.Value * delta;
            }).ScheduleParallel();
    }
}
