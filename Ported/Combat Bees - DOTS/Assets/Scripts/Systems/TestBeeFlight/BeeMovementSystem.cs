using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
public partial class BeeMovementSystem : SystemBase
{
    private float SquaredTargetReach;

    protected override void OnCreate()
    {
        Entities.WithAll<Bee>().ForEach((in BeeTargets beeTargets) =>
        {
            SquaredTargetReach = beeTargets.TargetWithinReach * beeTargets.TargetWithinReach;
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        // If "SquaredTargetReach" is accessed directly, it can't be used in other threads
        // so it would have to be used in combination with ".Run()" and ".WithoutBurst()" in the "Entities...ForEach()"
        float squaredTargetReach = SquaredTargetReach;
        
        Entities.WithAll<Bee>().ForEach((ref Translation translation, ref BeeMovement beeMovement, ref BeeTargets beeTargets) =>
        {
            float3 delta = beeTargets.CurrentTarget - translation.Value;
            float squaredDistanceFromTarget = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

            // If within reach of the target, switch targets
            // (Using squared values to not have to calculate an expensive square root operation)
            // if (squaredDistanceFromTarget <= squaredTargetReach)
            // {
            //     if (math.all(beeTargets.CurrentTarget == beeTargets.LeftTarget)) // Because the comparison returns bool3
            //     {
            //         beeTargets.CurrentTarget = beeTargets.RightTarget;
            //     }
            //     else
            //     {
            //         beeTargets.CurrentTarget = beeTargets.LeftTarget;
            //     }
            // }

            // Add velocity towards the current target
            beeMovement.Velocity += delta * beeMovement.Speed;
            
            // Move bee closer to the target
            translation.Value += beeMovement.Velocity * deltaTime;
        }).Run();
    }
}