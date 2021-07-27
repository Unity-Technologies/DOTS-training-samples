using Unity.Entities;
using Unity.Transforms;

public class PlayerAimerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        Entities
            .ForEach((ref Translation translation, in Movement movement) =>
            {
                // TODO: player aim logic using mouse input 
            }).ScheduleParallel();
    }
}