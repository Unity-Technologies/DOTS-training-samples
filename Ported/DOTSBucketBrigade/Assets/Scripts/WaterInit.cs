using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class WaterInit : SystemBase
{
    private EntityQuery m_BucketsQuery;
    private EndInitializationEntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        RequireSingletonForUpdate<BucketBrigadeConfig>();
        
        m_BucketsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ ComponentType.ReadOnly<BucketTag>() },
        });

        m_Barrier =
            World.DefaultGameObjectInjectionWorld.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (m_BucketsQuery.CalculateChunkCount() > 0)
            return;
        
        var config = GetSingleton<BucketBrigadeConfig>();
        var prefabs = GetSingleton<SpawnerConfig>();

        var rand = new Unity.Mathematics.Random();
        rand.InitState();

        float2 gridSize = (float2)config.GridDimensions * config.CellSize;

        for (int i = 0; i < config.NumberOfBuckets; ++i)
        {
            var bucket = EntityManager.Instantiate(prefabs.BucketPrefab);
            var bucketPos = GetComponent<Translation>(bucket);
            bucketPos.Value = new float3(rand.NextFloat(gridSize.x), 0, rand.NextFloat(gridSize.y));
            SetComponent(bucket, bucketPos);

            var bucketColor = GetComponent<BucketColor>(bucket);
            bucketColor.Value = math.float4(config.EmptyBucketColor.r, config.EmptyBucketColor.g,
                config.EmptyBucketColor.b, 1f);
            SetComponent(bucket, bucketColor);
        }

        var ecb = m_Barrier.CreateCommandBuffer().ToConcurrent();

        Entities.WithAll<WaterSource>().ForEach((Entity entity, int entityInQueryIndex, in NonUniformScale scale)
            =>
        {
            ecb.AddComponent(entityInQueryIndex, entity, new InitialScale {Value = scale.Value});
        }).ScheduleParallel();
        
        m_Barrier.AddJobHandleForProducer(Dependency);
    }
}
