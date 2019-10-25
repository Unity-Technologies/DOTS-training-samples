using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System.Runtime.CompilerServices;

namespace HighwayRacers
{
    // This is by far the most expensive system here.
    // Probably because of the cache misses in the hash tables.
    //
    // If I had to redo this, I would toss the hash tables, and implement
    // as an ordered array per lane.  Round the car positions to the size of
    // a single car, and insert in the right slot.
    // Quickly find my slot and look at the neighbours.
    //
    public struct HighwaySpacePartition
    {
        struct BucketEntry
        {
            public float2 Pos; // (avgDistanceFromOrigin, lane)
            public float Speed;
        }
        NativeMultiHashMap<int, BucketEntry> HashMap;

        int NumBuckets;
        float BucketLength;
        float TrackLength;

        int NextBucket(int index) { return (index + 1) % NumBuckets; }
        int PrevBucket(int index) { return (index + NumBuckets - 1) % NumBuckets; }

        public void Create(
            float trackLength, float desiredBucketLength, int capacity, Allocator allocator)
        {
            if (HashMap.IsCreated && HashMap.Capacity == capacity)
                HashMap.Clear();
            else
            {
                Dispose();
                HashMap = new NativeMultiHashMap<int, BucketEntry>(capacity, allocator);
            }
            TrackLength = trackLength;
            NumBuckets = math.max(1, (int)math.round(trackLength / math.max(1, desiredBucketLength)));
            BucketLength = trackLength / NumBuckets;
        }

        public void Dispose()
        {
            if (HashMap.IsCreated)
                HashMap.Dispose();
        }

        public struct ParallelWriter
        {
            public ParallelWriter(HighwaySpacePartition sp)
            {
                writer = sp.HashMap.AsParallelWriter();
                mBucketLength = sp.BucketLength;
            }

            NativeMultiHashMap<int, BucketEntry>.ParallelWriter writer;
            float mBucketLength;
            int GetBucketIndex(float pos) { return (int)math.floor(pos / mBucketLength); }

            // Pos units are average lane distance from origin
            public void AddCar(float pos, float lane, float speed)
            {
                writer.Add(GetBucketIndex(pos), new BucketEntry
                    { Pos = new float2(pos, lane), Speed = speed });
            }
        }

        // Use this to add cars
        public ParallelWriter AsParallelWriter() { return new ParallelWriter(this); }

        // Pos units are average lane distance from origin
        int GetBucketIndex(float pos) { return (int)math.floor(pos / BucketLength); }

        // Won't return a distance longer than half TrackLength
        float GetSignedDistanceBetweenCars(float fromPos, float toPos)
        {
            float halfTrackLen = TrackLength * 0.5f;
            var fromPosB = math.select(fromPos, fromPos - TrackLength, fromPos >= halfTrackLen);
            var toPosB = math.select(toPos, toPos - TrackLength, toPos >= halfTrackLen);
            float d = toPos - fromPos;
            return math.select(d, toPosB - fromPosB, math.abs(d) >= halfTrackLen);
        }

        // Distances are in average lane space
        public struct QueryResult
        {
            public enum ItemFlags
            {
                None = 0,
                Front = 1,
                FL = 2,
                FR = 4,
                RL = 8,
                RR = 16,
                All = Front | FL | FR | RL | RR
            };
            public ItemFlags FilledItems;

            public bool HasAll { get { return FilledItems == ItemFlags.All; } }
            public bool HasFront { get { return (FilledItems & ItemFlags.Front) != 0; } }

            public struct Item
            {
                public float2 Value;
                public float Distance { get { return Value.x; } }
                public float Speed { get { return Value.y; } }
            }
            public Item NearestFrontMyLane;
            public Item NearestFrontLeft;
            public Item NearestFrontRight;
            public Item NearestRearLeft;
            public Item NearestRearRight;
        }

        // Distances are in average lane space
        public QueryResult GetNearestCars(
            float pos, float lane, float maxDistance, float carSize)
        {
            int myBucket = GetBucketIndex(pos);
            float distanceInBucket = pos - (myBucket * BucketLength);
            float maxDistanceFront = maxDistance + carSize;
            float maxDistanceRear = maxDistance + carSize;
            int numFwdBuckets = GetBucketIndex(pos + maxDistanceFront) - myBucket;
            int numRearBuckets = myBucket - GetBucketIndex(pos - maxDistanceRear);

            // How far are we really looking?
            maxDistanceFront = numFwdBuckets * (BucketLength + 1) - distanceInBucket;
            maxDistanceRear = numRearBuckets * BucketLength + distanceInBucket;
            var result = new QueryResult();
            result.NearestFrontMyLane.Value
                = result.NearestFrontLeft.Value
                = result.NearestFrontRight.Value
                = result.NearestRearLeft.Value
                = result.NearestRearRight.Value = new float2(float.MaxValue, 0);

            var myPos = new float2(pos, lane);
            var myLane = math.round(lane);

            int bucketIndex = myBucket;
            for (int b = 0; b <= numFwdBuckets; ++b)
            {
                var bucket = HashMap.GetValuesForKey(bucketIndex);
                while (bucket.MoveNext())
                {
                    var e = bucket.Current;
                    if (math.all(e.Pos == myPos))
                        continue; // ignore myself
                    float dCenters = GetSignedDistanceBetweenCars(pos, e.Pos.x);
                    float d = math.sign(dCenters) * math.max(0, math.abs(dCenters) - carSize); // subtract the car size
                    bool inFront = dCenters >= 0;

                    // My lane
                    var laneDelta = myLane - e.Pos.y;
                    bool replace = inFront && math.abs(laneDelta) <= kSameLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestFrontMyLane, QueryResult.ItemFlags.Front, e, d, replace);
                    // Right lane front
                    replace = inFront && laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestFrontRight, QueryResult.ItemFlags.FR, e, d, replace);
                    // Right lane rear
                    laneDelta = myLane - e.Pos.y;
                    replace = !inFront && b == 0
                        && laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestRearRight, QueryResult.ItemFlags.RR, e, d, replace);

                    // Left lane front
                    laneDelta = e.Pos.y - myLane;
                    replace = inFront && laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestFrontLeft, QueryResult.ItemFlags.FL, e, d, replace);
                    // Left lane rear
                    replace = !inFront && b == 0
                        && laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestRearLeft, QueryResult.ItemFlags.RL, e, d, replace);
                }
                // Early out if done
                if (result.HasAll)
                    return result;
                bucketIndex = NextBucket(bucketIndex);
            }

            bucketIndex = PrevBucket(myBucket);
            for (int b = 0; b < numRearBuckets; ++b)
            {
                var bucket = HashMap.GetValuesForKey(bucketIndex);
                while (bucket.MoveNext())
                {
                    var e = bucket.Current;
                    float d = GetSignedDistanceBetweenCars(pos, e.Pos.x);
                    d = math.sign(d) * math.max(0, math.abs(d) - carSize); // subtract the car size

                    // Right lane front
                    var laneDelta = myLane - e.Pos.y;
                    bool replace = laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestRearRight, QueryResult.ItemFlags.RR, e, d, replace);
                    // Left lane rear
                    laneDelta = e.Pos.y - myLane;
                    replace = laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance;
                    result.FilledItems |= ReplaceIfCloser(
                        ref result.NearestRearLeft, QueryResult.ItemFlags.RL, e, d, replace);
                }
                // Early out if done
                if (result.HasAll)
                    return result;
                bucketIndex = PrevBucket(bucketIndex);
            }
            return result;
        }

        const float kSameLaneDistance = 0.8f;
        const float kNeighbourLaneDistance = 1.5f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        QueryResult.ItemFlags ReplaceIfCloser(
            ref QueryResult.Item item, QueryResult.ItemFlags itemFlag,
            BucketEntry e, float distance, bool replace)
        {
            replace = replace && item.Distance > distance;
            item.Value = math.select(item.Value, new float2(distance, e.Speed), replace);
            return (QueryResult.ItemFlags)math.select(0, (int)itemFlag, replace);
        }
    }
}
