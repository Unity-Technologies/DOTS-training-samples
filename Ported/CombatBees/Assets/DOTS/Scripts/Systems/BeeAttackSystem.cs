using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BeeAttackSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;
    private EntityQuery QueryAliveBees;


    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        // Query list of opposing team bees
        EntityQueryDesc queryAliveBeesDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(IsBee) },
            None = new ComponentType[] { typeof(IsDead) }
        };
        
        QueryAliveBees = GetEntityQuery(queryAliveBeesDesc);

    }
    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();

        // 1. in Perception, bees that are not returning (or dead), they can decide to target another bee to attack
        // 2. once per second, if an opposing team bee is within a threshold distance change target to that bee for attack
        // 3. move bee towards target
        // 4. attack when bee is with a specified distance of target - start timer
        // 4.1 velocity increased for the duration of the attack (timer?)
        // 4.2 if during attack (timer) bee comes with a specified distance of target then target is killed
        // 4.3 attack ends on timer

        // job to 'start attacks'
        // for all bees that are not already attacking, returning, dead or are not cooling down from a previous attack
        //    test against their aggression for 'chance' that they look to attack
        //    iterate over all opposing team bees to look for closest bee to attack and set as target of attack (expensive)
        //    'start the attack' => start the attack timer, add IsAttack tag, set target of attack

        // TODO how to move this into parallel worker threads?
        // safety complains that the threads will outlive the QueryAliveBees results

        var random = new Random(1234);
        var cdfeTeam = GetComponentDataFromEntity<Team>(true);
        var cdfeTranslation = GetComponentDataFromEntity<Translation>(true);
        using (var aliveBees = QueryAliveBees.ToEntityArray(Allocator.TempJob))
        {
            if (aliveBees.Length > 0)
            {
                Entities
                    .WithAll<IsBee>()
                    .WithNone<IsDead, IsAttacking, IsReturning>()
                    .WithNone<AttackCooldown>()
                    .WithReadOnly(cdfeTeam)
                    .WithReadOnly(cdfeTranslation)
                    .ForEach((Entity entity, ref Speed speed, in Aggression aggression, in Team team, in Translation translation) =>
                    {
                        // test if bee is aggressive anough to look to start an attack
                        if (aggression.Value > random.NextFloat(0, 1))
                        {
                            Entity? closestOpposingBee = null;
                            float closestsqDistance = 9999999999f;
                            // iterate over all opposing team bees to look for closest bee to attack and set as target of attack (expensive)
                            foreach (var bee in aliveBees)
                            {
                                // only consider bees on opposing team
                                if (team.Id != cdfeTeam[bee].Id)
                                {
                                    var distancesqToBee = math.distancesq(translation.Value, cdfeTranslation[bee].Value);
                                    if (distancesqToBee < closestsqDistance)
                                    {
                                        closestsqDistance = distancesqToBee;
                                        closestOpposingBee = bee;
                                    }
                                }
                            }

                            // if we found a bee to attack, start the attack
                            if (closestOpposingBee != null)
                            {
                                ecb.AddComponent<AttackTimer>(entity, new AttackTimer { Value = 2f });
                                ecb.AddComponent<IsAttacking>(entity);
                                if (HasComponent<Target>(entity))
                                {
                                    ecb.SetComponent<Target>(entity, new Target { Value = (Entity)closestOpposingBee });
                                }
                                else
                                {
                                    ecb.AddComponent<Target>(entity, new Target { Value = (Entity)closestOpposingBee });
                                    ecb.AddComponent<TargetPosition>(entity);
                                }
                                speed.MaxSpeedValue *= 2;
                            }
                        }
                    }).Run();
            }
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
            .WithAll<IsAttacking>()
            .ForEach((Entity entity, ref AttackTimer attackTimer, ref Speed speed, in Translation translation, in TargetPosition targetPosition, in Target target) =>
            {
                attackTimer.Value -= timeDeltaTime;
                bool attackOver = attackTimer.Value <= 0 || HasComponent<IsDead>(entity);

                // if the attack is in progress, test if close enough to kill opposing bee
                if (!attackOver && math.distancesq(translation.Value, targetPosition.Value) < 1)
                {
                    attackOver = true;
                    // kill the opposing bee, drop any carried resource, add gravity
                    Entity opposingBee = target.Value;
                    ecb.AddComponent<IsDead>(opposingBee);
                    ecb.AddComponent<HasGravity>(opposingBee);
                    if (HasComponent<IsReturning>(opposingBee))
                    {
                        Entity resourceCarriedByOpposingBee = GetComponent<Target>(opposingBee).Value;
                        ecb.RemoveComponent<IsCarried>(resourceCarriedByOpposingBee);
                        ecb.AddComponent<HasGravity>(resourceCarriedByOpposingBee);
                        ecb.RemoveComponent<IsReturning>(opposingBee);
                        ecb.RemoveComponent<Target>(opposingBee);
                        ecb.RemoveComponent<TargetPosition>(opposingBee);
                    }
                }

                // if the attack is over, end the attack
                if (attackOver)
                {
                    ecb.RemoveComponent<IsAttacking>(entity);
                    ecb.RemoveComponent<AttackTimer>(entity);
                    ecb.AddComponent(entity, new AttackCooldown { Value = 2f });
                    ecb.RemoveComponent<Target>(entity);
                    speed.MaxSpeedValue /= 2;
                }
            }).Run();

        // decrement attack cool downs
        Entities
            .ForEach((Entity entity, ref AttackCooldown attackCooldown) =>
            {
                attackCooldown.Value -= timeDeltaTime;
                if (attackCooldown.Value < 0)
                {
                    ecb.RemoveComponent<AttackCooldown>(entity);
                }
            }).Run();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
