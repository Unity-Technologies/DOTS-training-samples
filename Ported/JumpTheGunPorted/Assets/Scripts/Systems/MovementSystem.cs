using Unity.Entities;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .ForEach((ref Translation translation, in Movement movement) =>
            {
                // TODO: parabola logic
                //translation.Value.x = (float) ((time + movement.Offset) % 100) - 50f;
            }).ScheduleParallel();
    }
}