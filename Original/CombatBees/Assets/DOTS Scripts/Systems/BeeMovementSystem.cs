using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(FoodMovementSystem))]
public class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovementToTarget")
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref PhysicsData physicsData, in Translation translation, in TargetPosition t, in MoveSpeed speed) =>
            {
                var directionVector = t.Value - translation.Value;
                var destVector = math.normalize(directionVector);
                physicsData.a += destVector * speed.Value;

            }).ScheduleParallel();
        
        Entities
            .WithName("BeeJitter")
            .WithAll<BeeTag>()
            .ForEach((ref PhysicsData physicsData, ref RandomComponent random) =>
            {
                var jitter = random.Value.NextFloat3Direction() * random.Value.NextFloat() * zones.FlightJitter;
                physicsData.a += jitter;
        }).ScheduleParallel();
    }
}