using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeAttackingMovement : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    protected override void OnUpdate()
    {
        float dashMultiplier = 3f;
        BeeProperties beeProperties = GetSingleton<BeeProperties>();
        float deltaTime = World.Time.DeltaTime;
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;

        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref Translation translation, ref BeeTargets beeTargets, ref Velocity velocity, ref BeeStatus beeStatus, ref RandomState randomState) =>
        {
            // We are getting the BeeDead component here because we get compile error when we put it in lambda expression 
            var isBeeDead = GetComponent<BeeDead>(entity).Value;
            if (beeStatus.Value == Status.Attacking && !isBeeDead)
            {
                float3 delta = beeTargets.CurrentTargetPosition - translation.Value;
                float distanceFromTarget = delta.DistanceToFloat();
                
                if (distanceFromTarget < beeProperties.KillingReach +2)
                {
                    // Set current bee to Idle
                    beeStatus.Value = Status.Idle;
                    
                    // Kill the bee
                    var enemyComponent = GetComponent<BeeDead>(beeTargets.EnemyTarget);
                    enemyComponent.Value = true;
                    SetComponent(beeTargets.EnemyTarget, enemyComponent);
                    
                    beeTargets.EnemyTarget = Entity.Null; // for the bee that is attacking, not dying
                    
                    
                    return;
                }
                if (distanceFromTarget < beeProperties.AttackDashReach) // Enemy reached
                {
                    // Kill and go home
                    velocity.Value += delta * (beeProperties.ChaseForce * dashMultiplier / distanceFromTarget);
                }
              
                // Add velocity towards the current target
                velocity.Value += delta * (beeProperties.ChaseForce / distanceFromTarget);
                
                // Add random jitter
                float3 randomJitter = randomState.Value.NextFloat3(-1f, 1f);
                velocity.Value += randomJitter * beeProperties.FlightJitter;
                
                // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                velocity.Value *= 1f - beeProperties.Damping;
                // Move bee closer to the target
                translation.Value += velocity.Value * deltaTime;

                // Clamp the position within the field container
                translation.Value = math.clamp(translation.Value, containerMinPos, containerMaxPos);
            }
        }).Run();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        var spawningData = GetSingleton<BloodSpawningProperties>();
        Entities.WithAll<BeeTag>().ForEach((Entity entity,int entityInQueryIndex, ref BeeStatus beeStatus, ref BeeDead beeDead, ref RandomState randomState,ref Falling falling ,in Translation translation) =>
        {
            if (beeDead.Value)
            {
                beeStatus.Value = Status.Dead;
                falling.shouldFall = true;
                
                // To make the bloodparticles only spawn once 
                if (!beeDead.AnimationStarted)
                {
                    beeDead.AnimationStarted = true;
                    for (int i = 0; i < spawningData.amountParticles; i++)
                    {
                        Entity e = ecb.Instantiate(entityInQueryIndex,spawningData.bloodEntity);
                        ecb.AddComponent(entityInQueryIndex,e, new Translation
                        {
                            Value = translation.Value 
                        });
                        ecb.AddComponent(entityInQueryIndex,e, new Falling
                        {
                            timeToLive = randomState.Value.NextFloat(1, 3),
                            shouldFall = true
                        });
                        ecb.AddComponent(entityInQueryIndex,e, new BloodTag()
                        {
                            
                        });
                        
                        float x = randomState.Value.NextFloat(-5, 5);
                        float y = randomState.Value.NextFloat(0, 5);
                        float z = randomState.Value.NextFloat(-5, 5);
                        ecb.AddComponent(entityInQueryIndex,e, new Velocity()
                        {
                            Value = new float3(x, y, z),
                        });
                    }
                }
            }
        }).ScheduleParallel();
        sys.AddJobHandleForProducer(this.Dependency);
    }
}

