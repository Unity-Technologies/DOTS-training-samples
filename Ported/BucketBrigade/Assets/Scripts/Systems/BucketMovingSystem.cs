using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BucketMovingSystem : ISystem
{
   private EntityQuery m_FetcherQuery;
   private EntityQuery m_FiremenQuery;
   private bool m_ResetFiremen;

    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<Bucket>();
        
        m_FetcherQuery = state.GetEntityQuery(typeof(Fetcher), typeof(Translation));
        m_FiremenQuery = state.GetEntityQuery(typeof(Fireman), typeof(Translation));
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) 
    {
        var fetchers = m_FetcherQuery.ToComponentDataArray<Fetcher>(Allocator.Temp);
        var fetcherTranslations = m_FetcherQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        
        var firemen = m_FiremenQuery.ToComponentDataArray<Fireman>(Allocator.Temp);
        var firemanTranslations = m_FiremenQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        foreach (var bucket in SystemAPI.Query<BucketAspect>())
        {
            MoveBucketWithFetchers(fetchers, fetcherTranslations, bucket);
            MoveBucketWithFiremen(firemen, firemanTranslations, bucket);
        }

        if (m_ResetFiremen)
        {
            foreach (var fireman in SystemAPI.Query<FiremanAspect>())
            {
                fireman.FiremanState = FiremanState.Reset;
            }
            m_ResetFiremen = false;
        }
    }

    private void MoveBucketWithFiremen(NativeArray<Fireman> firemen, NativeArray<Translation> firemenTranslations, BucketAspect bucket)
    {
        bucket.NbOfFiremen = firemen.Length;
        for (int idx = 0; idx < firemen.Length; idx++)
        {
            var fireman = firemen[idx];
            if (fireman.State == FiremanState.Stopped)
            {
                if (bucket.CurrentFiremanIdx == idx)
                {
                    if(bucket.Interactions == BucketInteractions.Drop)
                    {
                        if (bucket.FillLevel > 0)
                        {
                            bucket.Position = firemenTranslations[idx].Value + new float3(0, 1.0f, 0);
                            bucket.CurrentFiremanIdx++;
                            bucket.Interactions = BucketInteractions.Chaining;
                            break;
                        }
                    }
                    else if (bucket.Interactions == BucketInteractions.Chaining)
                    {
                        if (bucket.AwaiterCount == 10)
                        {
                            bucket.AwaiterCount = 0;
                            bucket.Position = firemenTranslations[idx].Value + new float3(0, 1.0f, 0);
                            bucket.CurrentFiremanIdx++;

                            if (bucket.CurrentFiremanIdx == bucket.NbOfFiremen)
                            {
                                bucket.CurrentFiremanIdx = 0;
                                bucket.Interactions = BucketInteractions.Pour;
                                m_ResetFiremen = true;
                            }
                            break;
                        }
                        else
                        {
                            bucket.AwaiterCount++;
                        }
                    }
                }
            }
        }
    }

    private void MoveBucketWithFetchers(NativeArray<Fetcher> fetchers, NativeArray<Translation> fetcherTranslations, BucketAspect bucket)
    { 
        for (int idx = 0; idx < fetchers.Length; idx++)
        {
            var fetcher = fetchers[idx];
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
                    bucket.Interactions = BucketInteractions.Drop;
                }
            }
        }
    }
}