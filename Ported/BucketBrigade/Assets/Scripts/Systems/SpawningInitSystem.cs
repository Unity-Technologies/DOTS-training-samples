using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FireInitSystem))]
public class SpawningInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity entity, in InitDataActors actorTunning, in Translation spawnerTranslation) =>
        {
            for (int i = 0; i < actorTunning.ActorCount; i++)
            {
                Entity e = ecb.Instantiate(actorTunning.ActorPrefab);
                ecb.SetComponent(e, new Translation(){Value = spawnerTranslation.Value});
            }
            ecb.RemoveComponent<InitDataActors>(entity);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in InitBucketData bucketTunning, in Translation spawnerTranslation) =>
        {
            for (int i = 0; i < bucketTunning.BucketCount; i++)
            {
                Entity e = ecb.Instantiate(bucketTunning.BucketPrefab);
                ecb.SetComponent(e, new Translation(){Value = spawnerTranslation.Value});
            }
            ecb.RemoveComponent<InitBucketData>(entity);
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

