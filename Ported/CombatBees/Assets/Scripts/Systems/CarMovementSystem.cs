using Unity.Entities;
using Unity.Transforms;

public class CarMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Time is a field of SystemBase, and SystemBase is a class. This prevents
        // using it in a job, so we have to fetch ElapsedTime and store it in a local
        // variable. This local variable can then be used in the job.
        var time = Time.ElapsedTime;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .ForEach((ref Translation translation) =>
            {
                translation.Value.x = (float)time;
            }).ScheduleParallel();
    }
}