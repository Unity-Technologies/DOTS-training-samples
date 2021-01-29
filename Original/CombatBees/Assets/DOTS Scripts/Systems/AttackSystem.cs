using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeMovementSystem))]
public class AttackSystem : SystemBase
{
    private ComponentTypes typesToRemoveFromAttacker;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SpawnZones>();
        
        ComponentType[] types = new ComponentType[]
        {
            ComponentType.ReadOnly<MoveTarget>(),
            ComponentType.ReadOnly<TargetPosition>(),
            ComponentType.ReadOnly<AttackingBeeTag>(),
        };

        typesToRemoveFromAttacker = new ComponentTypes(types);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var spawnZones = GetSingleton<SpawnZones>();
        var typesToRemoveFromAttacker = this.typesToRemoveFromAttacker;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Attack")
            .WithAll<BeeTag, AttackingBeeTag>()
            .ForEach((Entity e, ref Translation translation, ref TargetPosition t, ref RandomComponent rng, ref PhysicsData physics, in AttackData attackData, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(attackData.Distance, t.Value, translation.Value))
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
                    ecb.RemoveComponent(e, typesToRemoveFromAttacker);

                    // Create a blood droplet
                    var numberOfBloodDrops = rng.Value.NextInt(2, 5);
                    for (int i = 0; i < numberOfBloodDrops; ++i)
                    {
                        var blood = ecb.Instantiate(spawnZones.BloodPrefab);
                        ecb.AddComponent(blood, new RandomComponent() { Value = new Random(rng.Value.NextUInt()) });
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
                else
                {
                    var direction = t.Value - translation.Value;
                    var norm = math.normalize(direction);
                    physics.a += norm * attackData.Force;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}