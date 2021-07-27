using Unity.Entities;
using Unity.Transforms;

public class TankAimerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        /*Entities
            .ForEach((ref Translation translation, TODO) =>
            {
                // TODO: tank aim logic
            }).ScheduleParallel();*/
    }
}