// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
using Unity.Entities;
using Unity.Transforms;

public class CarMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .ForEach((ref Translation translation, in CarMovement movement) =>
            {
                translation.Value.x = (float)((time + movement.Offset) % 100) - 50f;
            }).ScheduleParallel();
    }
}