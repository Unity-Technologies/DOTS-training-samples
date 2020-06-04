using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class Water : SystemBase
{
    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();
        var time = Time.DeltaTime;
        Entities.WithAll<WaterSource>().ForEach((ref WaterLevel level)
            =>
        {
            level.Level = math.clamp(level.Level + (config.WaterSourceRefillRate * time), 0f, level.Capacity);
        }).ScheduleParallel();
    }
}