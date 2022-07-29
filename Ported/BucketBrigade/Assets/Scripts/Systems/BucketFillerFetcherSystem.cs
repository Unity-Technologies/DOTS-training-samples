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
        m_BucketQuery = state.GetEntityQuery(typeof(BucketInfo), typeof(Translation), typeof(BucketId));
        m_fireLineQuery = state.GetEntityQuery(typeof(FireFighterLine));
        m_deltaDistance = 0.0001f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moveJob = new BucketFillerFetcherTarget
        {
            bucketTranslations = m_BucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob),
            bucketIds = m_BucketQuery.ToComponentDataArray<BucketId>(Allocator.TempJob),
            fireLines = m_fireLineQuery.ToComponentDataArray<FireFighterLine>(Allocator.TempJob),
            deltaDistance = m_deltaDistance
        };

        moveJob.Schedule();
    }

    [BurstCompile]
    partial struct BucketFillerFetcherTarget : IJobEntity
    {
        public NativeArray<Translation> bucketTranslations;
        [ReadOnly]
        public NativeArray<BucketId> bucketIds;
        [ReadOnly]
        public NativeArray<FireFighterLine> fireLines;
        public float deltaDistance;

        [BurstCompile]
        void Execute(in Translation translation, ref Target target, ref BucketFillerFetcher bucketFillerFetcher, in LineId lineID, ref BucketId bucketID)
        {
            if (bucketFillerFetcher.state == BucketFillerFetcher.BucketFillerFetcherState.GoToBucket)
            {
                var minDistance = float.MaxValue;
                var minDistanceIndex = 0;

                if (bucketTranslations.Length < 1)
                {
                    return;
                }

                for (var i = 0; i < bucketTranslations.Length; i++)
                {
                    var distance = math.distancesq(translation.Value.xz, bucketTranslations[i].Value.xz);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minDistanceIndex = i;
                    }
                }

                target.Value = bucketTranslations[minDistanceIndex].Value.xz;
                bucketID.Value = bucketIds[minDistanceIndex].Value;

                // Reached the bucket
                if (minDistance < deltaDistance)
                {
                    bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.GoToLake;
                }
            }
            else if (bucketFillerFetcher.state == BucketFillerFetcher.BucketFillerFetcherState.GoToLake)
            {
                foreach (var line in fireLines)
                {
                    if (line.LineId != lineID.Value)
                    {
                        continue;
                    }

                    target.Value = line.StartPosition;

                    // Reached lake
                    if (math.length(target.Value - translation.Value.xz) < deltaDistance)
                    {
                        bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.FillBucket;
                    }
                }
            }
            else
            {
                // TODO: wait for fill, go to different bucket
                bucketFillerFetcher.state = BucketFillerFetcher.BucketFillerFetcherState.GoToBucket;
            }

        }
    }
}

