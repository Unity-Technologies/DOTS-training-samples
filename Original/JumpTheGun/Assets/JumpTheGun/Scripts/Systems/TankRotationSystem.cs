using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial class TankRotationSystem : SystemBase
{
    private EntityQuery playerQuery;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerComponent>();
        playerQuery = GetEntityQuery(typeof(PlayerComponent));
    }
    protected override void OnUpdate()
    {
        float deltaTime = this.Time.DeltaTime;
        var playerEntity = playerQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);

        if (playerEntity.Length == 0)
            return; 

        if (!HasComponent<LocalToWorld>(playerEntity[0]))
            return;

        // Look up the entity data
        LocalToWorld targetTransform = GetComponent<LocalToWorld>(playerEntity[0]);

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

                
            }).Run();

               
    }
}
