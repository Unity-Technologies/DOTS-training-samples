using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class TankRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = this.Time.DeltaTime;

        Entities
            .ForEach((ref Rotation orientation,
            in LocalToWorld transform,
            in PlayerComponent target) =>
            {
                // Check to make sure the target Entity still exists and has
                // the needed component
                if (!HasComponent<LocalToWorld>(target.playerEntity))
                    return;

                // Look up the entity data
                LocalToWorld targetTransform
                    = GetComponent<LocalToWorld>(target.playerEntity);
                float3 targetPosition = targetTransform.Position;

                // Calculate the rotation
                float3 displacement = targetPosition - transform.Position;
                float3 upReference = new float3(0, 1, 0);
                quaternion lookRotation =
                    quaternion.LookRotationSafe(displacement, upReference);

                orientation.Value =
                    math.slerp(orientation.Value, lookRotation, deltaTime);
            }).ScheduleParallel();
    }
}
