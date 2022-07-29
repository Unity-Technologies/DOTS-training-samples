using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class TankRotationSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponent>();
    }
    protected override void OnUpdate()
    {
        float deltaTime = this.Time.DeltaTime;

        // Check to make sure the target Entity still exists and has
        // the needed component
        if (!HasComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>()))
            return;

        // Look up the entity data
        LocalToWorld targetTransform = GetComponent<LocalToWorld>(GetSingletonEntity<PlayerComponent>());

        Entities
            .WithAll<Tank>()
            .ForEach((TransformAspect transform) =>
            {
                 float3 targetPosition = targetTransform.Position;

                              // Calculate the rotation
                                 float3 displacement = targetPosition - transform.Position;
                                 float3 upReference = new float3(0, 1, 0);
                                 displacement.y = transform.Position.y;
                                 quaternion lookRotation = quaternion.LookRotationSafe(displacement, upReference);

                                transform.Rotation = math.slerp(transform.Rotation, lookRotation, deltaTime);

              /*float3 diff = targetPosition - transform.Position;
                float angle = math.atan2(diff.x, diff.z);
                transform.Rotation = quaternion.Euler(0, angle * UnityEngine.Mathf.Rad2Deg, 0);*/


            }).Schedule();

               
    }
}
