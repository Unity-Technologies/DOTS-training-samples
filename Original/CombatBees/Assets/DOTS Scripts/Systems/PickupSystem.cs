using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeMovementSystem))]
public class PickupSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
        RequireSingletonForUpdate<Randomizer>();
    }

    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Pickup")
            .WithAll<BeeTag>()
            .WithNone<CarriedFood>()
            .WithoutBurst()
            .ForEach((Entity e, ref Translation translation, ref TargetPosition t, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, t.Value, translation.Value))
                {
                    ecb.AddComponent( e, new CarriedFood() {Value = moveTarget.Value});
                    ecb.AddComponent( moveTarget.Value, new CarrierBee() {Value = e});

                    ecb.RemoveComponent<MoveTarget>( e);
                    var random = GetSingleton<Randomizer>();
                    AABB zone;
                    if (EntityManager.HasComponent<Team1>(e))
                    {
                        zone = zones.Team1Zone;
                    }
                    else if (EntityManager.HasComponent<Team2>(e))
                    {
                        zone = zones.Team2Zone;
                    }
                    else
                    {
                        zone = new AABB();
                    }
                    t.Value = random.Random.NextFloat3(zone.Min, zone.Max);
                    SetSingleton(random);
                }
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}