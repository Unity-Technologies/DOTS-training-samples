using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


partial class PillCollisionSystem : SystemBase
{
    EntityQuery Query;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerData>();
    }

    protected override void OnUpdate()
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        PlayerData playerData = SystemAPI.GetSingleton<PlayerData>();
        TransformAspect playerTransform = SystemAPI.GetAspectRO<TransformAspect>(playerEntity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        float3 playerPosition = playerTransform.Position;
        Entities.WithAll<RotationComponent>().ForEach((Entity entity, TransformAspect transform) =>
        {
            var distance = math.distancesq(playerPosition, transform.Position);
            if(distance < 0.1f)
            {
                ecb.DestroyEntity(entity);
                playerData.PillsCollected++;
                ecb.SetComponent<PlayerData>(playerEntity, playerData);
            }

        }).Schedule();

        this.Dependency.Complete();
        ecb.Playback(this.EntityManager);
        ecb.Dispose();
    }
}
