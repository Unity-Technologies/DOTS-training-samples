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
        state.RequireForUpdate<Bucket>();
        
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
                    //UnityEngine.Debug.Log($"Test: { fetcher.TargetPickUpIdx }");

                    if (fetcher.CurrentState == FetcherState.ArriveAtBucket ||
                        fetcher.CurrentState == FetcherState.MoveTowardsWater)
                    {
                        bucket.Interactions = BucketInteractions.PickedUp;
                        bucket.Position = fetcherTranslations[idx].Value + new float3(0, 1.0f, 0);
                    }
                    else if (fetcher.CurrentState == FetcherState.FillingBucket)
                    {
                        bucket.FillLevel = 1.0f;
                    }
                }
            }
        }
    }
}