using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial class PillCollisionSystem : SystemBase
{
    EntityQuery Query;

    protected override void OnCreate()
    {
   
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

        /*
        // The amount of rotation around Y required to do 360 degrees in 2 seconds.
        var rotation = quaternion.RotateX(state.Time.DeltaTime * math.PI);

        // The classic C# foreach is what we often refer to as "Idiomatic foreach" (IFE).
        // Aspects provide a higher level interface than directly accessing component data.
        // Using IFE with aspects is a powerful and expressive way of writing main thread code.
        // WithAll adds a constraint to the query, specifying that every entity should have such component.
        QueryEnumerable<TransformAspect> pills =  SystemAPI.Query<TransformAspect>().WithAll<RotationComponent>();

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        CharacterAspect player = SystemAPI.GetAspectRW<CharacterAspect>(playerEntity);

        EntityQueryMask mask = Query.GetEntityQueryMask();

        EntityCommandBuffer ecb = new EntityCommandBuffer();

        foreach(TransformAspect transform in pills)
        {
            
        }*/
    }
}
