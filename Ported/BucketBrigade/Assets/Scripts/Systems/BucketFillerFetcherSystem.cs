using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct BucketFillerFetcherSystem : ISystem
{
    EntityQuery m_BucketQuery;
    EntityQuery m_fireLineQuery;
    float m_deltaDistance;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_BucketQuery = state.GetEntityQuery(typeof(BucketInfo), typeof(Translation), typeof(BucketId), typeof(Volume));
        m_fireLineQuery = state.GetEntityQuery(typeof(FireFighterLine));
        m_deltaDistance = 0.000001f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BucketConfig>();

        var moveJob = new BucketFillerFetcherTarget
        {
            BucketTranslations = m_BucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob),
            BucketIds = m_BucketQuery.ToComponentDataArray<BucketId>(Allocator.TempJob),
            BucketVolumes = m_BucketQuery.ToComponentDataArray<Volume>(Allocator.TempJob),
            BucketInfos = m_BucketQuery.ToComponentDataArray<BucketInfo>(Allocator.TempJob),
            FireLines = m_fireLineQuery.ToComponentDataArray<FireFighterLine>(Allocator.TempJob),
            Delta = m_deltaDistance,
            Capacity = config.Capacity
        };

        moveJob.Schedule();
    }

    [BurstCompile]
    partial struct BucketFillerFetcherTarget : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Translation> BucketTranslations;
        [ReadOnly]
        public NativeArray<BucketId> BucketIds;
        [ReadOnly]
        public NativeArray<FireFighterLine> FireLines;
        [ReadOnly]
        public NativeArray<Volume> BucketVolumes;
        [ReadOnly]
        public NativeArray<BucketInfo> BucketInfos;
        public float Delta;
        public float Capacity;

        [BurstCompile]
        void Execute(in Translation translation, ref Target target, ref BucketFillerFetcher bucketFillerFetcher, in LineId lineID, ref BucketId bucketID)
        {
            if (bucketFillerFetcher.state == BucketFillerFetcher.BucketFillerFetcherState.GoToBucket)
            {
                var minDistance = float.MaxValue;
                var minDistanceIndex = 0;
                var validBucket = false;

                if (BucketTranslations.Length < 1)
                {
                    return;
                }

                for (var i = 0; i < BucketTranslations.Length; i++)
                {
                    // Ignore full and taken buckets
                    if (BucketVolumes[i].Value > (Capacity - Delta) || BucketInfos[i].IsTaken)
                    {
                        continue;
                    }

                    var distance = math.distancesq(translation.Value.xz, BucketTranslations[i].Value.xz);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistanceIndex = i;
                        validBucket = true;
                    }
                }

                if (validBucket)
                {
                    target.Value = BucketTranslations[minDistanceIndex].Value.xz;
                    bucketID.Value = BucketIds[minDistanceIndex].Value;
                }
                else
                {
                    target.Value = translation.Value.xz;
                }

                // Reached the bucket
                if (minDistance < 0.1)
                {
                    bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.GoToLake;
                }
            }
            else if (bucketFillerFetcher.state == BucketFillerFetcher.BucketFillerFetcherState.GoToLake)
            {
                foreach (var line in FireLines)
                {
                    if (line.LineId != lineID.Value)
                    {
                        continue;
                    }

                    target.Value = line.StartPosition;

                    // Reached lake
                    if (math.length(target.Value - translation.Value.xz) < 0.1)
                    {
                        bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.FillBucket;
                    }
                }
            }
            else
            {
                var bucketIndex = 0;

                for (var i = 0; i < BucketIds.Length; i++)
                {
                    if (bucketID.Value == BucketIds[i].Value)
                    {
                        bucketIndex = i;
                        break;
                    }
                }

                // Full bucket
                if (BucketVolumes[bucketIndex].Value > (Capacity - Delta))
                {
                    bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.GoToBucket;
                }
            }

        }
    }
}

