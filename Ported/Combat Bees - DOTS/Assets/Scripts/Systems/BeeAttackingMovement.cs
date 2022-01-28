using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeAttackingMovement : SystemBase
{
    protected override void OnUpdate()
    {
        float dashMultiplier = 3f;
        BeeProperties beeProperties = GetBeeProperties();
        float deltaTime = World.Time.DeltaTime;
        float3 containerMinPos = GetSingleton<Container>().MinPosition;
        float3 containerMaxPos = GetSingleton<Container>().MaxPosition;

        Entities.WithAll<BeeTag>().ForEach((ref Translation translation, ref BeeTargets beeTargets, ref Velocity velocity, ref BeeStatus beeStatus) =>
        {
            if (beeStatus.Value == Status.Attacking)
            {
                float3 delta = beeTargets.CurrentTargetPosition - translation.Value;
                float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                if (distanceFromTarget < beeProperties.TargetReach)
                {
                    beeStatus.Value = Status.Idle;
                    beeTargets.EnemyTarget = Entity.Null;
                    // TODO: Set other bee to dead
                    return;
                }
                
                if (distanceFromTarget < beeProperties.AttackDashReach) // Enemy reached
                {
                    // Kill and go home
                    velocity.Value += delta * (beeProperties.ChaseForce * dashMultiplier / distanceFromTarget);
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