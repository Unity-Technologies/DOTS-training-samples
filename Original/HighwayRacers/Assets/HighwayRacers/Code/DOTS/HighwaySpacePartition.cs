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
        float TrackLength;

        int NextBucket(int index) { return (index + 1) % NumBuckets; }
        int PrevBucket(int index) { return (index + NumBuckets - 1) % NumBuckets; }

        public void Create(
            float trackLength, float desiredBucketLength, int capacity, Allocator allocator)
        {
            Dispose();

            TrackLength = trackLength;
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
                mBucketLength = sp.BucketLength;
            }

            NativeMultiHashMap<int, BucketEntry>.ParallelWriter writer;
            float mBucketLength;
            int GetBucketIndex(float pos) { return (int)math.floor(pos / mBucketLength); }

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

        // Won't return a distance longer than half TrackLength
        float GetSignedDistanceBetweenCars(float fromPos, float toPos)
        {
            float halfTrackLen = TrackLength * 0.5f;
            fromPos = math.select(fromPos, fromPos - TrackLength, fromPos >= halfTrackLen);
            toPos = math.select(toPos, toPos - TrackLength, toPos >= halfTrackLen);
            return toPos - fromPos;
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
            int carId, float pos, float lane, float maxDistance, float carSize)
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
            result.NearestFrontMyLane.Distance
                = result.NearestFrontLeft.Distance
                = result.NearestFrontRight.Distance
                = result.NearestRearLeft.Distance
                = result.NearestRearRight.Distance = float.MaxValue;

            var myLane = math.round(lane);

            int bucketIndex = myBucket;
            for (int b = 0; b <= numFwdBuckets; ++b)
            {
                var bucket = HashMap.GetValuesForKey(bucketIndex);
                while (bucket.MoveNext())
                {
                    var e = bucket.Current;
                    if (e.CarID == carId)
                        continue; // ignore myself
                    float dCenters = GetSignedDistanceBetweenCars(pos, e.Pos.x);
                    float d = math.sign(dCenters) * math.max(0, math.abs(dCenters) - carSize); // subtract the car size
                    if (dCenters >= 0)
                        GatherFrontDistances(e, ref result, myLane, d);
                    if (dCenters < 0 && b == 0)
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
                    float d = GetSignedDistanceBetweenCars(pos, e.Pos.x);
                    if (d < 0)
                    {
                        d = math.sign(d) * math.max(0, math.abs(d) - carSize); // subtract the car size
                        GatherRearDistances(e, ref result, myLane, d);
                    }
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
        void GatherFrontDistances(BucketEntry e, ref QueryResult result, float myLane, float d)
        {
            // My lane
            var laneDelta = myLane - e.Pos.y;
            if (math.abs(laneDelta) <= kSameLaneDistance)
            {
                if (result.NearestFrontMyLane.Distance > d)
                    result.NearestFrontMyLane
                        = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
                return;
            }
            // Right lane
            if (laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance)
            {
                if (result.NearestFrontRight.Distance > d)
                    result.NearestFrontRight
                        = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
                return;
            }
            // Left lane
            laneDelta = e.Pos.y - myLane;
            if (laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance)
            {
                if (result.NearestFrontLeft.Distance > d)
                    result.NearestFrontLeft
                        = new QueryResult.Item { CarId = e.CarID, Distance = d, Speed = e.Speed };
            }
        }

        // Returns true if this entry is handled
        void GatherRearDistances(BucketEntry e, ref QueryResult result, float myLane, float d)
        {
            // Right lane
            var laneDelta = myLane - e.Pos.y;
            if (laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance)
            {
                if (result.NearestRearRight.Distance > -d)
                    result.NearestRearRight
                        = new QueryResult.Item { CarId = e.CarID, Distance = -d, Speed = e.Speed };
                return;
            }
            // Left lane
            laneDelta = e.Pos.y - myLane;
            if (laneDelta > kSameLaneDistance && laneDelta < kNeighbourLaneDistance)
            {
                if (result.NearestRearLeft.Distance > -d)
                    result.NearestRearLeft
                        = new QueryResult.Item { CarId = e.CarID, Distance = -d, Speed = e.Speed };
            }
        }
    }
}
