using Unity.Entities;
using Unity.Transforms;

public class TankAimerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .ForEach((ref Translation translation, in Movement movement) =>
            {
                // TODO: tank aim logic
            }).ScheduleParallel();
    }
}