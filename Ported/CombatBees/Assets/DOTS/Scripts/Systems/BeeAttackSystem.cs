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
    private EntityQuery QueryAliveBees;


    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        // Query list of opposing team bees
        var queryAliveBeesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsBee) },
            None = new ComponentType[] { typeof(IsDead) }
        };
        
        QueryAliveBees = GetEntityQuery(queryAliveBeesDesc);

    }
    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();
        var pecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        // job to 'start attacks'
        // for all bees that are not already attacking, returning, dead or are not cooling down from a previous attack
        //    test against their aggression for 'chance' that they look to attack
        //    iterate over all opposing team bees to look for closest bee to attack and set as target of attack (expensive)
        //    'start the attack' => start the attack timer, add IsAttack tag, set target of attack

        // TODO how to move this into parallel worker threads?
        // safety complains that the threads will outlive the QueryAliveBees results

        var aliveBees = QueryAliveBees.ToEntityArray(Allocator.TempJob);
        
        if (aliveBees.Length > 0)
        {
            Entities
                .WithName("StartAttack")
                .WithAll<IsBee>()
                .WithNone<IsDead, IsAttacking, IsReturning>()
                .WithNone<AttackCooldown>()
                .WithDisposeOnCompletion(aliveBees)
                .ForEach((Entity entity, ref Speed speed, in Aggression aggression, in Team team, in Translation translation) =>
                {
                    // test if bee is aggressive enough to look to start an attack
                    if (aggression.Value > 0.5)
                    {
                        var closestOpposingBee = Entity.Null;
                        var closestsqDistance = 9999999999f;
                        // iterate over all opposing team bees to look for closest bee to attack and set as target of attack (expensive)
                        foreach (var bee in aliveBees)
                        {
                            var beeTeam = GetComponent<Team>(bee);
                            
                            // only consider bees on opposing team
                            if (team.Id != beeTeam.Id)
                            {
                                var beeTranslation = GetComponent<Translation>(bee);
                                var distancesqToBee = math.distancesq(translation.Value, beeTranslation.Value);
                                
                                if (distancesqToBee < closestsqDistance)
                                {
                                    closestsqDistance = distancesqToBee;
                                    closestOpposingBee = bee;
                                }
                            }
                        }

                        // if we found a bee to attack, start the attack
                        if (closestOpposingBee != Entity.Null)
                        {
                            ecb.AddComponent<IsAttacking>(entity);
                            ecb.AddComponent(entity, new AttackTimer { Value = 2f });
                            ecb.AddComponent(entity, new Target { Value = closestOpposingBee });
                            ecb.AddComponent<TargetPosition>(entity);
                            
                            speed.MaxValue *= 2;
                        }
                    }
                }).Schedule();
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
                    
                    pecb.AddComponent<IsDead>(entityInQueryIndex, opposingBee);
                    pecb.AddComponent<HasGravity>(entityInQueryIndex, opposingBee);

                    pecb.SetComponent(entityInQueryIndex, opposingBee, new Velocity { Value = math.normalize(velocity.Value) });
                    
                    pecb.SetComponent(entityInQueryIndex, opposingBee, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4(1, 0, 0, 1)
                    });

                    if (HasComponent<Target>(opposingBee))
                    {
                        if (HasComponent<IsReturning>(opposingBee))
                        {
                            var opposingBeeResource = GetComponent<Target>(opposingBee).Value;
                        
                            pecb.RemoveComponent<IsReturning>(entityInQueryIndex, opposingBee);
                            pecb.RemoveComponent<IsCarried>(entityInQueryIndex, opposingBeeResource);
                            pecb.AddComponent<HasGravity>(entityInQueryIndex, opposingBeeResource);
                        }
                        
                        pecb.RemoveComponent<Target>(entityInQueryIndex, opposingBee);
                    }
                }

                // if the attack is over, end the attack
                if (attackOver)
                {
                    pecb.RemoveComponent<IsAttacking>(entityInQueryIndex, entity);
                    pecb.RemoveComponent<AttackTimer>(entityInQueryIndex, entity);
                    pecb.RemoveComponent<Target>(entityInQueryIndex, entity);
                    
                    pecb.AddComponent(entityInQueryIndex, entity, new AttackCooldown { Value = 1 });
                    
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
                    pecb.RemoveComponent<AttackCooldown>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
