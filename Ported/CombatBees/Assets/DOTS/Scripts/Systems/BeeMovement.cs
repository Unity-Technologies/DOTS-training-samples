using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(BeeUpdateGroup))]
[UpdateAfter(typeof(BeePerception))]
public class BeeMovement : SystemBase
{
    const float maxRotation = 360;
    const float distanceMaxSpeed = 4;
    
    [ReadOnly]
    private ComponentDataFromEntity<Translation> cdfe;

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        // Update all TargetPositions with current position of Target (deterministic!)
        Entities
            .WithName("UpdateTargetPosition")
            .WithNone<IsReturning, IsDead>()
            .ForEach((ref TargetPosition targetPosition, in Target target) =>
            {
                var targetEntity = target.Value;
                var targetTranslation = GetComponent<Translation>(targetEntity);
                
                targetPosition.Value = targetTranslation.Value;
            }).ScheduleParallel();

        // Move bees that are targeting (a Resource or Base or opposing bee) towards the target's position
        Entities
            .WithName("MoveToTarget")
            .WithAll<IsBee>()
            .WithNone<IsDead>()
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

                var direction = math.normalize(velocity.Value);
                var distanceToTarget = math.lengthsq(newLookAt);
                var currentSpeed = math.lerp(speed.MinValue, speed.MaxValue, math.clamp(distanceToTarget / distanceMaxSpeed, 0, 1));
                
                velocity.Value = direction * currentSpeed;

                //translation.Value += velocity.Value * deltaTime;
                //translation.Value.y = math.max(translation.Value.y, 0.05f);
            }).ScheduleParallel();
    }
}
