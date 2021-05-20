using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
[UpdateAfter(typeof(BeePerception))]
public class BeeMovement : SystemBase
{
    const float maxRotation = 180f;
    const float distanceMaxSpeed = 4f;
    
    [ReadOnly]
    private ComponentDataFromEntity<Translation> cdfe;

    protected override void OnUpdate()
    {
        cdfe = GetComponentDataFromEntity<Translation>(true);
        
        var deltaTime = Time.DeltaTime;
        
        // Update all TargetPositions with current position of Target (deterministic!)
        Entities
            .WithNone<IsReturning>()
            .WithoutBurst()
            .ForEach((ref TargetPosition targetPosition, in Target target) =>
            {
                targetPosition.Value = cdfe[target.Value].Value;
            }).Run();

        // TODO when a bee dies it's target must be removed
        // Move bees that are targetting (a Resource or Base) towards the target's position
        Entities
            .WithAll<IsBee>()
            .ForEach((ref Translation translation, ref Velocity velocity, in TargetPosition targetPosition, in Speed speed) =>
            {
                var currentPosition = translation.Value;
                var newLookAt = targetPosition.Value - currentPosition;

                var q = UnityEngine.Quaternion.FromToRotation(velocity.Value, newLookAt);
                q.ToAngleAxis(out var angle, out var axis);
                var deltaAngle = maxRotation * deltaTime;
                deltaAngle = angle > deltaAngle ? deltaAngle : angle;
                q = UnityEngine.Quaternion.AngleAxis(deltaAngle, axis);

                velocity.Value = math.rotate(q, velocity.Value);

                float3 direction = math.normalize(velocity.Value);

                float distanceToTarget = math.lengthsq(newLookAt);
                float currentSpeed = math.lerp(speed.MinSpeedValue, speed.MaxSpeedValue, math.clamp(distanceToTarget / distanceMaxSpeed, 0, 1));
                velocity.Value = direction * currentSpeed;

                translation.Value += velocity.Value * deltaTime;
                translation.Value.y = math.max(translation.Value.y, 0.05f);
            }).Schedule();
    }
}
