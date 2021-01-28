using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeMovementSystem))]
public class PickupSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
    }

    protected override void OnUpdate()
    {
        var zones = GetSingleton<SpawnZones>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        //This won't allow parallel scheduling. To do so we need to place a random number generator component in each bee,
        //and then initialize that generator uniquely for each spawned bee.
        var randomSeed = (uint) ((Time.DeltaTime + Time.ElapsedTime) * 10000000);
        var random = Random.CreateFromIndex(randomSeed);

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Pickup")
            .WithAll<BeeTag, FetchingFoodTag>()
            .WithNone<CarriedFood>()
            .ForEach((Entity e, ref Translation translation, ref TargetPosition t, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, t.Value, translation.Value))
                {
                    ecb.AddComponent( e, new CarriedFood() {Value = moveTarget.Value});
                    ecb.AddComponent( moveTarget.Value, new CarrierBee() {Value = e});

                    ecb.RemoveComponent<MoveTarget>( e);
                    ecb.RemoveComponent<FetchingFoodTag>( e);
                    AABB zone;
                    if (HasComponent<Team1>(e))
                    {
                        zone = zones.Team1Zone;
                    }
                    else if (HasComponent<Team2>(e))
                    {
                        zone = zones.Team2Zone;
                    }
                    else
                    {
                        zone = new AABB();
                    }
                    
                    t.Value = random.NextFloat3(zone.Min, zone.Max);
                }
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}