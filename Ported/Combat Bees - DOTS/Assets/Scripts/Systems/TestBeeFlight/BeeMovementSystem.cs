using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        
        Entities.WithAll<Bee>().ForEach((ref Translation translation, ref Rotation rotation, ref BeeMovement beeMovement, ref BeeTargets beeTargets) =>
        {
            float3 delta = beeTargets.CurrentTarget - translation.Value;
            float distanceFromTarget = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

            // If within reach of the target, switch targets
            if (distanceFromTarget < beeTargets.TargetReach)
            {
                // Used "math.all()" because the comparison returns bool3
                if (math.all(beeTargets.CurrentTarget == beeTargets.LeftTarget))
                {
                    beeTargets.CurrentTarget = beeTargets.RightTarget;
                    // Rotate 180 degrees
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(180)));
                }
                else
                {
                    beeTargets.CurrentTarget = beeTargets.LeftTarget;
                    // Rotate 180 degrees
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(180)));
                }
            }

            // Add velocity towards the current target
            // (not used because it was flying too far past the target)
            // beeMovement.Velocity += delta * beeMovement.Speed * deltaTime / distanceFromTarget;

            // Move bee closer to the target
            translation.Value += math.normalize(delta) * beeMovement.Speed * deltaTime; //beeMovement.Velocity;
        }).Run();
    }
}