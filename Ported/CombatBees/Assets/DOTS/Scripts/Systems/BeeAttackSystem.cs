using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
[UpdateAfter(typeof(BeePerception))]
public class BeeAttackSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;
    private EntityQuery QueryAliveYellowTeamBees;
    private EntityQuery QueryAliveBlueTeamBees;


    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        // Query list of opposing team bees
        var queryAliveYellowTeamBeesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsBee), typeof(YellowTeam) },
            None = new ComponentType[] { typeof(IsDead) }
        };
        
        QueryAliveYellowTeamBees = GetEntityQuery(queryAliveYellowTeamBeesDesc);
        var queryAliveBlueTeamBeesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsBee), typeof(BlueTeam) },
            None = new ComponentType[] { typeof(IsDead) }
        };
        
        QueryAliveBlueTeamBees = GetEntityQuery(queryAliveBlueTeamBeesDesc);

    }
    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        // job to 'start attacks'
        // for all bees that are not already attacking, returning, dead or are not cooling down from a previous attack
        //    test against their aggression for 'chance' that they look to attack
        //    iterate over all opposing team bees to look for closest bee to attack and set as target of attack (expensive)
        //    'start the attack' => start the attack timer, add IsAttack tag, set target of attack

        // TODO how to move this into parallel worker threads?
        // safety complains that the threads will outlive the QueryAliveBees results

        var aliveYellowTeamBees = QueryAliveYellowTeamBees.ToEntityArray(Allocator.TempJob);
        var aliveBlueTeamBees = QueryAliveBlueTeamBees.ToEntityArray(Allocator.TempJob);
        var random = Utils.GetRandom();
        
        if (aliveYellowTeamBees.Length > 0)
        {
            Entities
                .WithName("StartAttackBlueTeam")
                .WithAll<IsBee, BlueTeam>()
                .WithNone<IsDead, IsAttacking, IsReturning>()
                .WithNone<AttackCooldown>()
                .WithReadOnly(aliveYellowTeamBees)
                .WithDisposeOnCompletion(aliveYellowTeamBees)
                .ForEach((int entityInQueryIndex, Entity entity, ref Speed speed, in Aggression aggression, in Team team, in Translation translation) =>
                {
                // test if bee is aggressive enough to look to start an attack
                if (aggression.Value > random.NextFloat(0, 1))
                    {
                        // pick a random opposing bee to attack
                        ecb.AddComponent<IsAttacking>(entityInQueryIndex, entity);
                        ecb.AddComponent(entityInQueryIndex, entity, new AttackTimer { Value = 2f });
                        ecb.AddComponent(entityInQueryIndex, entity, new Target { Value = aliveYellowTeamBees[random.NextInt(0, aliveYellowTeamBees.Length)] });
                        ecb.AddComponent<TargetPosition>(entityInQueryIndex, entity);
                            
                        speed.MaxValue *= 2;
                    }
                }).ScheduleParallel();
        }
        if (aliveBlueTeamBees.Length > 0)
        {
            Entities
                .WithName("StartAttackYellowTeam")
                .WithAll<IsBee, YellowTeam>()
                .WithNone<IsDead, IsAttacking, IsReturning>()
                .WithNone<AttackCooldown>()
                .WithReadOnly(aliveBlueTeamBees)
                .WithDisposeOnCompletion(aliveBlueTeamBees)
                .ForEach((int entityInQueryIndex, Entity entity, ref Speed speed, in Aggression aggression, in Team team, in Translation translation) =>
                {
                    // test if bee is aggressive enough to look to start an attack
                    if (aggression.Value > random.NextFloat(0, 1))
                    {
                        // pick a random opposing bee to attack
                        ecb.AddComponent<IsAttacking>(entityInQueryIndex, entity);
                        ecb.AddComponent(entityInQueryIndex, entity, new AttackTimer { Value = 2f });
                        ecb.AddComponent(entityInQueryIndex, entity, new Target { Value = aliveBlueTeamBees[random.NextInt(0, aliveBlueTeamBees.Length)] });
                        ecb.AddComponent<TargetPosition>(entityInQueryIndex, entity);
                            
                        speed.MaxValue *= 2;
                    }
                }).ScheduleParallel();
        }

        // job to 'process attacks'
        // for all bees that are attacking
        //    speed boost during attack
        //    decrement the attack timer to see if the attack is over => set attack cool timer
        //    test whether close enough to target bee to 'kill' target
        //       set state to reflect opposing bee is killed
        //       end attack
        
        var timeDeltaTime = Time.DeltaTime;

        Entities
            .WithName("ProcessAttack")
            .WithAll<IsAttacking>()
            .ForEach((int entityInQueryIndex, Entity entity, ref AttackTimer attackTimer, ref Speed speed, in Translation translation, in TargetPosition targetPosition, in Target target, in Velocity velocity) =>
            {
                attackTimer.Value -= timeDeltaTime;
                
                var attackOver = attackTimer.Value <= 0 || HasComponent<IsDead>(entity);

                // if the attack is in progress, test if close enough to kill opposing bee
                if (!attackOver && math.distancesq(translation.Value, targetPosition.Value) < 1)
                {
                    attackOver = true;
                    
                    // kill the opposing bee, drop any carried resource, add gravity
                    var opposingBee = target.Value;
                    
                    ecb.AddComponent<IsDead>(entityInQueryIndex, opposingBee);
                    ecb.AddComponent<HasGravity>(entityInQueryIndex, opposingBee);

                    ecb.SetComponent(entityInQueryIndex, opposingBee, new Velocity { Value = math.normalize(velocity.Value) });
                    
                    ecb.SetComponent(entityInQueryIndex, opposingBee, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(1, 0, 0, 1)
                    });

                    if (HasComponent<Target>(opposingBee))
                    {
                        if (HasComponent<IsReturning>(opposingBee))
                        {
                            var opposingBeeResource = GetComponent<Target>(opposingBee).Value;
                        
                            ecb.RemoveComponent<IsReturning>(entityInQueryIndex, opposingBee);
                            ecb.RemoveComponent<IsCarried>(entityInQueryIndex, opposingBeeResource);
                            ecb.AddComponent<HasGravity>(entityInQueryIndex, opposingBeeResource);
                        }
                        
                        ecb.RemoveComponent<Target>(entityInQueryIndex, opposingBee);
                    }
                }

                // if the attack is over, end the attack
                if (attackOver)
                {
                    ecb.RemoveComponent<IsAttacking>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<AttackTimer>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                    
                    ecb.AddComponent(entityInQueryIndex, entity, new AttackCooldown { Value = 1 });
                    
                    speed.MaxValue /= 2;
                }
            }).ScheduleParallel();

        // decrement attack cool downs
        Entities
            .WithName("CooldownAttack")
            .ForEach((int entityInQueryIndex, Entity entity, ref AttackCooldown attackCooldown) =>
            {
                attackCooldown.Value -= timeDeltaTime;
                
                if (attackCooldown.Value < 0)
                {
                    ecb.RemoveComponent<AttackCooldown>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
