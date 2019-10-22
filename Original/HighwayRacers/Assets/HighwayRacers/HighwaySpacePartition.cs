using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace HighwayRacers
{

    public struct HighwaySpacePartition
    {
        struct BucketEntry
        {
            public int CarID;
            public float2 Pos; // (avgDistanceFromOrigin, lane)
            public float Speed;
        }
        NativeMultiHashMap<int, BucketEntry> HashMap;

        int NumBuckets;
        float BucketLength;
        float HalfTrackLength;

        int NextBucket(int index) { return (index + 1) % NumBuckets; }
        int PrevBucket(int index) { return (index + NumBuckets - 1) % NumBuckets; }

        public void Create(
            float trackLength, float desiredBucketLength, int capacity, Allocator allocator)
        {
            Dispose();

            HalfTrackLength = trackLength / 2;
            NumBuckets = math.max(1, (int)math.round(trackLength / desiredBucketLength));
            BucketLength = trackLength / NumBuckets;

            HashMap = new NativeMultiHashMap<int, BucketEntry>(capacity, allocator);
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
                BucketLength = sp.BucketLength;
            }

            NativeMultiHashMap<int, BucketEntry>.ParallelWriter writer;
            float BucketLength;
            int GetBucketIndex(float pos) { return (int)math.floor(pos / BucketLength); }

            // Pos units are average lane distance from origin
            public void AddCar(
                int carId, float pos, float lane, float speed)
            {
                writer.Add(GetBucketIndex(pos), new BucketEntry
                    { CarID = carId, Pos = new float2(pos, lane), Speed = speed });
            }
        }

        // Use this to add cars
        public ParallelWriter AsParallelWriter() { return new ParallelWriter(this); }

        // Pos units are average lane distance from origin
        int GetBucketIndex(float pos) { return (int)math.floor(pos / BucketLength); }

        // Won't return a distance longer than HalfTrackLength
        float GetDistance(float fromPos, float toPos)
        {
            float d = toPos - fromPos;
            return math.select(
                d,
                math.select(toPos, toPos - (2 * HalfTrackLength), toPos > HalfTrackLength)
                    - math.select(fromPos, fromPos - (2 * HalfTrackLength), fromPos > HalfTrackLength),
                math.abs(d) >= HalfTrackLength);
        }

        // Distances are in average lane space
        public struct QueryResult
        {
            public struct Item
            {
                public int CarId;
                public float Distance;
                public float Speed;
            }
            public Item NearestFrontMyLane;
            public Item NearestFrontLeft;
            public Item NearestFrontRight;
            public Item NearestRearLeft;
            public Item NearestRearRight;
        }

        // Distances are in average lane space
        public QueryResult GetNearestCars(
            float pos, float lane, float maxDistanceFront, float maxDistanceRear)
        {
            int myBucket = GetBucketIndex(pos);
            float distanceInBucket = pos - (myBucket * BucketLength);
            int numFwdBuckets = GetBucketIndex(pos + maxDistanceFront) - myBucket;
            int numRearBuckets = myBucket - GetBucketIndex(pos - maxDistanceRear);

            // How far are we really looking?
            maxDistanceFront = numFwdBuckets * (BucketLength + 1) - distanceInBucket;
            maxDistanceRear = numRearBuckets * BucketLength + distanceInBucket;
            var result = new QueryResult();

            var myLane = math.round(lane);

            int bucketIndex = myBucket;
            for (int b = 0; b <= numFwdBuckets; ++b)
            {
                var bucket = HashMap.GetValuesForKey(bucketIndex);
                while (bucket.MoveNext())
                {
                    var e = bucket.Current;
                    float d = GetDistance(pos, e.Pos.x);
                    if (!GatherFrontDistances(e, ref result, myLane, d) && b == 0)
                        GatherRearDistances(e, ref result, myLane, d);
                }
                // Early out if done
                if (IsComplete(ref result))
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
                    float d = GetDistance(pos, e.Pos.x);
                    GatherRearDistances(e, ref result, myLane, d);
                }
                // Early out if done
                if (IsComplete(ref result))
                    return result;
                bucketIndex = PrevBucket(bucketIndex);
            }
            return result;
        }

        const float kSameLaneDistance = 0.8f;
        const float kNeighbourLaneDistance = 1.5f;

        bool IsComplete(ref QueryResult result)
        {
            return result.NearestFrontMyLane.CarId != 0
                && result.NearestFrontLeft.CarId != 0
                && result.NearestFrontRight.CarId != 0
                && result.NearestRearLeft.CarId != 0
                && result.NearestRearRight.CarId != 0;
        }

        // Returns true if this entry is handled
        bool GatherFrontDistances(BucketEntry e, ref QueryResult result, float myLane, float d)
        {
            // Only consider objects in front
            if (d < 0)
                return false;

            // My lane
            if (math.abs(e.Pos.y - myLane) < kSameLaneDistance)
            {
                if (result.NearestFrontMyLane.CarId == 0 || result.NearestFrontMyLane.Distance > d)
                    result.NearestFrontMyLane = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
                return true;
            }
            // Right lane
            if (myLane - e.Pos.y < kNeighbourLaneDistance)
            {
                if (result.NearestFrontRight.CarId == 0 || result.NearestFrontRight.Distance > d)
                    result.NearestFrontRight = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
                return true;
            }
            // Left lane
            if (e.Pos.y - myLane < kNeighbourLaneDistance)
            {
                if (result.NearestFrontLeft.CarId == 0 || result.NearestFrontLeft.Distance > d)
                    result.NearestFrontLeft = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
                return true;
            }
            return false;
        }

        // Returns true if this entry is handled
        bool GatherRearDistances(BucketEntry e, ref QueryResult result, float myLane, float d)
        {
            // Only consider objects in rear
            if (d >= 0)
                return false;

            // Right lane
            if (myLane - e.Pos.y < kNeighbourLaneDistance)
            {
                if (result.NearestRearRight.CarId == 0 || result.NearestRearRight.Distance > -d)
                    result.NearestRearRight = new QueryResult.Item { CarId = e.CarID, Distance = -d, Speed = e.Speed };
                return true;
            }
            // Left lane
            if (e.Pos.y - myLane < kNeighbourLaneDistance)
            {
                if (result.NearestRearLeft.CarId == 0 || result.NearestRearLeft.Distance > -d)
                    result.NearestRearLeft = new QueryResult.Item { CarId = e.CarID, Distance = -d, Speed = e.Speed };
                return true;
            }
            return false;
        }
    }
}
