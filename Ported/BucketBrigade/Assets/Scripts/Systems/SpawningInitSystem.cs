using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(FireInitSystem))]
public class SpawningInitSystem : SystemBase
{
    Random random = new Random(1000);

    protected override void OnUpdate()
    {
        Entity tuningDataEntity = GetSingletonEntity<TuningData>();
        TuningData tuningData = EntityManager.GetComponentData<TuningData>(tuningDataEntity);
        
        EntityQuery eqFire = GetEntityQuery(ComponentType.ReadOnly<Fire>());
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithoutBurst().ForEach((Entity entity, in InitDataActors actorTunning) =>
        {
            for (int i = 0; i < actorTunning.ActorCount; i++)
            {
                Entity e = ecb.Instantiate(actorTunning.ActorPrefab);
                int x = random.NextInt((int)(-tuningData.GridSize.x * 0.5), (int)(tuningData.GridSize.x * 0.5));
                int z = random.NextInt((int)(-tuningData.GridSize.y * 0.5), (int)(tuningData.GridSize.y * 0.5));
                Translation position = new Translation()
                {
                    Value = new float3(x, 1, z)
                };
                ecb.SetComponent(e, position);
            }
            ecb.RemoveComponent<InitDataActors>(entity);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach((Entity entity, in InitBucketData bucketTunning) =>
        {
            for (int i = 0; i < bucketTunning.BucketCount; i++)
            {
                Entity e = ecb.Instantiate(bucketTunning.BucketPrefab);
                int x = random.NextInt((int)(-tuningData.GridSize.x * 0.5), (int)(tuningData.GridSize.x * 0.5));
                int z = random.NextInt((int)(-tuningData.GridSize.y * 0.5), (int)(tuningData.GridSize.y * 0.5));
                Translation position = new Translation()
                {
                    Value = new float3(x, 1, z)
                };
                ecb.SetComponent(e, position);
            }
            ecb.RemoveComponent<InitBucketData>(entity);
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

