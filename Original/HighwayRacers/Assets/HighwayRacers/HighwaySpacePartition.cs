using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace HighwayRacers
{
    public struct HighwaySpacePartition
    {
        struct Bucket
        {
            public struct Entry
            {
                public int CarID;
                public float2 Pos; // (avgDistanceFromOrigin, lane)
                public float Speed;
            }
            public NativeArray<Entry> Entries;
            public int NumUsed;
        }

        NativeArray<Bucket> Buckets;
        float BucketLength;
        float HalfTrackLength;

        int NextBucket(int index) { return (index + 1) % Buckets.Length; }
        int PrevBucket(int index) { return (index + Buckets.Length - 1) % Buckets.Length; }

        public void Create(float trackLength, float desiredBucketLength, float carLength)
        {
            Dispose();

            // Buckets must be big enough to hold a full set of cars
            HalfTrackLength = trackLength / 2;
            int numBuckets = math.max(1, (int)math.round(trackLength / desiredBucketLength));
            BucketLength = trackLength / numBuckets;
            int bucketSize = (int)math.ceil(BucketLength / carLength) * HighwayConstants.NUM_LANES;

            Buckets = new NativeArray<Bucket>(numBuckets, Allocator.Persistent);
            for (int i = 0; i < numBuckets; ++i)
                Buckets[i] = new Bucket
                    { Entries = new NativeArray<Bucket.Entry>(bucketSize, Allocator.Persistent) };
        }

        public void Dispose()
        {
            for (int i = 0; i < Buckets.Length; ++i)
                Buckets[i].Entries.Dispose();
            Buckets.Dispose();
        }

        int GetBucketIndex(float pos)
        {
            return (int)math.floor(pos / BucketLength);
        }

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

        // olsPos and newPos units are average lane distance from origin
        public void MoveCar(
            int carId, float oldPos, float newPos, float newLane, float newSpeed)
        {
            int oldIndex = GetBucketIndex(oldPos);
            int newIndex = GetBucketIndex(newPos);
            Bucket bucket;

            // Remove from old bucket
            if (oldIndex != newIndex)
            {
                bucket = Buckets[oldIndex];
                for (int i = 0; i < bucket.NumUsed; ++i)
                {
                    var e = bucket.Entries[i];
                    if (e.CarID == carId)
                    {
                        e.CarID = 0;
                        bucket.Entries[i] = e;
                        if (i == bucket.NumUsed - 1)
                        {
                            --bucket.NumUsed;
                            Buckets[oldIndex] = bucket;
                        }
                        break;
                    }
                }
            }

            // Add to new bucket
            bucket = Buckets[newIndex];
            bool found = false;
            var pos = new Vector2(newPos, newLane);
            for (int i = 0; i < bucket.NumUsed && !found; ++i)
            {
                var e = bucket.Entries[i];
                if (e.CarID == carId)
                {
                    bucket.Entries[i] = new Bucket.Entry { CarID = carId, Pos = pos, Speed = newSpeed };
                    found = true;
                }
            }
            if (!found)
            {
                bucket.Entries[bucket.NumUsed++] = new Bucket.Entry { CarID = carId, Pos = pos, Speed = newSpeed };
                Buckets[newIndex] = bucket;
            }
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
                var bucket = Buckets[bucketIndex];
                for (int i = 0; i < bucket.NumUsed; ++i)
                {
                    var e = bucket.Entries[i];
                    if (e.CarID != 0)
                    {
                        float d = GetDistance(pos, e.Pos.x);
                        if (!GatherFrontDistances(e, ref result, myLane, d) && b == 0)
                            GatherRearDistances(e, ref result, myLane, d);
                    }
                }
                // Early out if done
                if (IsComplete(ref result))
                    return result;
                bucketIndex = NextBucket(bucketIndex);
            }

            bucketIndex = PrevBucket(myBucket);
            for (int b = 0; b < numRearBuckets; ++b)
            {
                var bucket = Buckets[bucketIndex];
                for (int i = 0; i < bucket.NumUsed; ++i)
                {
                    var e = bucket.Entries[i];
                    if (e.CarID != 0)
                    {
                        float d = GetDistance(pos, e.Pos.x);
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
        bool GatherFrontDistances(Bucket.Entry e, ref QueryResult result, float myLane, float d)
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
        bool GatherRearDistances(Bucket.Entry e, ref QueryResult result, float myLane, float d)
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
