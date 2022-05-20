using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BucketMovingSystem : ISystem
{
   private EntityQuery m_FetcherQuery;

    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<FetcherTarget>();
        
        m_FetcherQuery = state.GetEntityQuery(typeof(Fetcher), typeof(Translation));
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        var fetchers = m_FetcherQuery.ToComponentDataArray<Fetcher>(Allocator.Temp);
        var fetcherTranslations = m_FetcherQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        for (int idx = 0; idx < fetchers.Length; idx++)
        {
            var fetcher = fetchers[idx];
            foreach (var bucket in SystemAPI.Query<BucketAspect>())
            {
                if (fetcher.TargetPickUp != Entity.Null
                    && fetcher.TargetPickUp.Equals(bucket.Self))
                {
                    bucket.Position = fetcherTranslations[idx].Value + new float3(0, 1.0f, 0);
                }
            }
        }
    }
}