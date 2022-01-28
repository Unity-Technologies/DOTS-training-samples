using Combatbees.Testing.Maria;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public partial class BeeAttackingMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float dashMultiplier = 3f;
        BeeProperties beeProperties = GetBeeProperties();
        float deltaTime = World.Time.DeltaTime;
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;

        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref Translation translation, ref BeeTargets beeTargets, ref Velocity velocity, ref BeeStatus beeStatus, ref RandomState randomState) =>
        {
            if (beeStatus.Value == Status.Attacking)
            {
                Debug.Log("Attacking movement starting");
                float3 delta = beeTargets.CurrentTargetPosition - translation.Value;
                float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                
                if (distanceFromTarget < beeProperties.TargetReach + 2)
                {
                    // Set current bee to Idle
                    beeStatus.Value = Status.Idle;
                    
                    // Kill the bee
                    var enemyComponent = GetComponent<BeeDead>(beeTargets.EnemyTarget);
                    enemyComponent.Value = true;
                    SetComponent(beeTargets.EnemyTarget, enemyComponent);

                    beeTargets.EnemyTarget = Entity.Null;

                    return;
                }
                if (distanceFromTarget < beeProperties.AttackDashReach) // Enemy reached
                {
                    // Kill and go home
                    velocity.Value += delta * (beeProperties.ChaseForce * dashMultiplier / distanceFromTarget);
                    Debug.Log("Started Dashing");
                }
                else
                {
                    Debug.Log("Just going slowly to the target bee");
                    // Add velocity towards the current target
                    velocity.Value += delta * (beeProperties.ChaseForce / distanceFromTarget);

                    // Add random jitter
                    float3 randomJitter = randomState.Random.NextFloat3(-1f, 1f);
                    velocity.Value += randomJitter * beeProperties.FlightJitter;

                    // Apply damping (also limits velocity so that it does not keep increasing indefinitely)
                    velocity.Value *= 1f - beeProperties.Damping;
                }
                
                // Move bee closer to the target
                translation.Value += velocity.Value * deltaTime;

                // Clamp the position within the field container
                translation.Value = math.clamp(translation.Value, containerMinPos, containerMaxPos);
            }
        }).Run();

        bool beeIsdead = false;
        // get the position of the Dead bee
        Entities.WithAll<BeeTag>().ForEach((Entity entity, ref BeeStatus beeStatus, in BeeDead beeDead, in Translation translation) =>
        {
            if(beeDead.Value)
            {
                beeStatus.Value = Status.Dead;
                beeIsdead = true;
            }
        }).Run();

        if (beeIsdead)
        {
            letBeeDisappearAndInitBloodParticles();
            moveBloodParticles();
            timeToLive();
        }
        
        
    }
    
    private BeeProperties GetBeeProperties()
    {
        BeeProperties beeProps = new BeeProperties();
        
        Entities.ForEach((in BeeProperties beeProperties) =>
        {
            beeProps = beeProperties;
        }).Run();

        return beeProps;
    }
    
    private void letBeeDisappearAndInitBloodParticles(){
                float3 pos = new float3(0);
                float steps = 0f;
                
                
                // initiate/spawn the blood particles
                Entities.ForEach((Entity entity,ref BeeDead beeDead, in Translation translation) =>
                {
                    pos = translation.Value;

                    var spawner = GetSingleton<BeeBloodSpawner>();
                    // To make the bloodparticles only spawn once 
                    if (!beeDead.AnimationStarted)
                    {
                        beeDead.AnimationStarted = true;
                        for (int i = 0; i < spawner.amountParticles; i++)
                        {
                            Entity e = EntityManager.Instantiate(spawner.bloodEntity);
                            EntityManager.SetComponentData(e, new Translation
                            {
                                Value = translation.Value + new float3(pos)    
                            });
                            float x = spawner.random.NextFloat(-3, 3);
                            float y = spawner.random.NextFloat(0, 3);
                            float z = spawner.random.NextFloat(-3, 3);
                            
                            EntityManager.SetComponentData(e, new BloodParticle
                            {
                                direction = new float3(x, y, z),
                                steps = spawner.steps,
                                timeToLive = spawner.random.NextFloat(0, 5)
                            });
                        }
                    }
                }).WithStructuralChanges().Run();
            
        }

        private void moveBloodParticles()
        {
            Entities.ForEach((Entity entity, ref BloodParticle bloodparticle, ref Translation translation) =>
            {
                if (0 <= bloodparticle.steps )
                {
                    bloodparticle.steps -= Time.DeltaTime;
                    translation.Value = translation.Value + new float3(bloodparticle.direction * Time.DeltaTime);
                }
                else
                {
                    if(translation.Value.y > 0) translation.Value.y += -10 * Time.DeltaTime;
                }
            }).WithStructuralChanges().Run();
        }

        private void timeToLive()
        {
            Entities.ForEach((Entity entity, ref BloodParticle bloodparticle, ref Translation translation) =>
            {
                if (0 <= bloodparticle.timeToLive )
                {
                    bloodparticle.timeToLive -= Time.DeltaTime;
                }
                else
                {
                    EntityManager.DestroyEntity(entity);
                }
            }).WithStructuralChanges().Run();   
        }
}


