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
                    beeStatus.Value = Status.Idle;
                    beeTargets.EnemyTarget = Entity.Null;
                    // TODO: Set other bee to dead
                    Debug.Log("Idle now");
                    
                    // QUESTION: We want to target the enemy bee and then assign 'dead' to it. However, this is not possible because 
                    // we are already beeTargets from the other bee.
                    var enemyEntity = GetComponent<BeeStatus>(beeTargets.EnemyTarget);
                    enemyEntity.Value = Status.Dead;
                    
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
}