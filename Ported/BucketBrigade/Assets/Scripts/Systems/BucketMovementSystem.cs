using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct BucketMovementSystem : ISystem
{
    EntityQuery m_BucketFillerFetcherQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_BucketFillerFetcherQuery = state.GetEntityQuery(typeof(BucketFillerFetcher), typeof(Translation), typeof(BucketId));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BucketConfig>();
        var configCell = SystemAPI.GetSingleton<TerrainCellConfig>();

        var bucketMoveJob = new BucketTravel
        {
            BucketFetcherTranslations = m_BucketFillerFetcherQuery.ToComponentDataArray<Translation>(Allocator.TempJob),
            BucketFetchers = m_BucketFillerFetcherQuery.ToComponentDataArray<BucketFillerFetcher>(Allocator.TempJob),
            BucketIds = m_BucketFillerFetcherQuery.ToComponentDataArray<BucketId>(Allocator.TempJob),
            Time = Time.deltaTime,
            FillSpeed = 1f,
            Capacity = config.Capacity,
            BaseBucketHeight = configCell.CellSize * 0.25f
        };

        bucketMoveJob.Schedule();
    }

    [BurstCompile]
    partial struct BucketTravel : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Translation> BucketFetcherTranslations;
        [ReadOnly]
        public NativeArray<BucketFillerFetcher> BucketFetchers;
        [ReadOnly]
        public NativeArray<BucketId> BucketIds;
        public float Time;
        public float FillSpeed;
        public float Capacity;
        public float BaseBucketHeight;

        [BurstCompile]
        void Execute(ref Translation translation, in BucketId bucketId, ref Volume volume)
        {
            for (var i = 0; i < BucketFetchers.Length; i++)
            {
                if (BucketIds[i].Value != bucketId.Value)
                {
                    continue;
                }

                // fetcher has bucket
                if (BucketFetchers[i].state == BucketFillerFetcher.BucketFillerFetcherState.GoToLake)
                {
                    translation.Value = BucketFetcherTranslations[i].Value + new float3 { y = 1 };
                }
                else if (BucketFetchers[i].state == BucketFillerFetcher.BucketFillerFetcherState.FillBucket)
                {
                    volume.Value += FillSpeed * Time;

                    if (volume.Value > Capacity)
                    {
                        volume.Value = Capacity;
                        translation.Value.y = BaseBucketHeight;
                    }
                }
            }
        }
    }
}
