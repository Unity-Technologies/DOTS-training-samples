using Unity.Entities;

public class AntExcitementSystem : SystemBase
{
    private const float NORMAL_EXCITEMENT = 0.3f;
    private const float HOLDING_RESOURCE_EXCITEMENT = 1f;

    private const float ANT_SPEED = 0.2f;

    protected override void OnUpdate()
    {
        Entities
            .WithAll<Ant>()
            .WithAll<HoldingResource>()
            .ForEach((ref Excitement excitement, in Speed speed) => 
            {
                excitement.Value = HOLDING_RESOURCE_EXCITEMENT * speed.Value / ANT_SPEED;

            }).ScheduleParallel();

        Entities
            .WithAll<Ant>()
            .WithNone<HoldingResource>()
            .ForEach((ref Excitement excitement, in Speed speed) =>
            {
                excitement.Value = NORMAL_EXCITEMENT * speed.Value / ANT_SPEED;

            }).ScheduleParallel();
    }
}
