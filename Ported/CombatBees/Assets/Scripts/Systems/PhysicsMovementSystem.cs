using Unity.Entities;
using Unity.Transforms;

public partial class PhysicsMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        float multiplier = 5;
        
        Entities
            .WithAll<Food>()
            .ForEach((ref Translation translation) =>
            {

                translation.Value.y = - (float) time * multiplier;

                
            }).ScheduleParallel();
    }
}