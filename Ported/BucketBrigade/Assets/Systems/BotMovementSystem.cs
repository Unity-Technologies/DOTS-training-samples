using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class BotMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, ref Translation translation, in PasserBot bot) =>
        {
            translation.Value = bot.PickupPosition;
        }).ScheduleParallel();
    }
}
