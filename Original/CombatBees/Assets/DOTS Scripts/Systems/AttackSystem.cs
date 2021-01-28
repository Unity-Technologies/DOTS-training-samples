using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeMovementSystem))]
public class AttackSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var spawnZones = GetSingleton<SpawnZones>();

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Attack")
            .WithAll<BeeTag, AttackingBeeTag>()
            .ForEach((Entity e, ref Translation translation, ref TargetPosition t, ref RandomComponent rng, in MoveTarget moveTarget, in PhysicsData physics) =>
            {
                if (MathUtil.IsWithinDistance(0.5f, t.Value, translation.Value))
                {
                    //We're close enough to the bee we want to attack. So turn it into a corpse.
                    ecb.RemoveComponent<BeeTag>(moveTarget.Value);
                    ecb.AddComponent<BeeCorpseTag>(moveTarget.Value);
                    ecb.AddComponent(moveTarget.Value, Lifetime.FromTimeRemaining(5));

                    //Keep in mind you have to also have the targeted bee drop whatever food it's carrying. Or rather,
                    //have the food recognize it's not being picked up anymore.
                    if (HasComponent<CarriedFood>(moveTarget.Value))
                    {
                        var carriedFood = GetComponent<CarriedFood>(moveTarget.Value).Value;
                        ecb.RemoveComponent<CarrierBee>(carriedFood);
                    }

                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb.RemoveComponent<AttackingBeeTag>(e);
                    ecb.RemoveComponent<MoveTarget>(e);
                    ecb.RemoveComponent<TargetPosition>(e);
                    
                    
                    // Create a blood droplet
                    var numberOfBloodDrops = rng.Value.NextInt(2, 5);
                    for (int i = 0; i < numberOfBloodDrops; ++i)
                    {
                        var blood = ecb.Instantiate(spawnZones.BloodPrefab);
                        ecb.AddComponent(blood, Lifetime.FromTimeRemaining(rng.Value.NextFloat(5, 15)));
                        ecb.SetComponent(blood, translation);
                        ecb.SetComponent(blood, new PhysicsData
                        {
                            a = new float3(),
                            v = physics.v + rng.Value.NextFloat3(new float3(math.length(physics.v) / 3)),
                            damping = 0,
                            kineticEnergyPreserved = new float3(),
                        });
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}