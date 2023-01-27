using Aspects;
using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utils;

namespace Jobs
{
    [WithAll(typeof(CarAspect))]
    [BurstCompile]
    public partial struct CarMovementJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Car> AllCars;

        [ReadOnly]
        public NativeArray<Lane> Lanes;

        [ReadOnly]
        public Config Config;

        public EntityCommandBuffer.ParallelWriter EntityParallelWriter;

        [ReadOnly]
        public float DeltaTime;

        [BurstCompile]
        private void Execute([ChunkIndexInQuery] int entityQueryIndex, ref CarAspect car)
        {

            float distanceToFrontCar = float.MaxValue;
            Car nearestFrontCar = AllCars[0];//assume there is a car and that we will have one close to us

            bool leftLaneOk = car.LaneNumber != Config.NumLanes - 1;//set to false if we are at the leftmost lane
            bool rightLaneOk = car.LaneNumber != 0;//set to false if we are at the rightmost lane

            // Don't change lane if in lane changing
            leftLaneOk = leftLaneOk && car.LaneChangeProgress > 1.0f;
            rightLaneOk = rightLaneOk && car.LaneChangeProgress > 1.0f;

            for (int otherIndex = 0; otherIndex < AllCars.Length; otherIndex++)
            {
                var other = AllCars[otherIndex];

                if (other.Index == car.Index) { continue;}

                // Update front car
                if (other.LaneNumber == car.LaneNumber)
                {
                    // there should always be a car in the front, either in this loop or in next loop
                    var distToOtherCarInLane = other.Distance - car.Distance;
                    if (distToOtherCarInLane < 0.0f)
                    {
                        distToOtherCarInLane = other.Distance + Lanes[car.LaneNumber].LaneLength - car.Distance;
                    }

                    if (distToOtherCarInLane < distanceToFrontCar)
                    {
                        distanceToFrontCar = distToOtherCarInLane;
                        nearestFrontCar = other;
                    }
                }

                // Check lane clearance on both sides
                float laneChangeClearance = 2 * Config.LaneChangeClearance;
                if (leftLaneOk)
                {
                    var leftLaneNumber = car.LaneNumber + 1;
                    var leftLane = Lanes[leftLaneNumber];
                    var laneDistAfterChanging = TransformationUtils.GetDistanceOnLaneChange(car.LaneNumber,
                        car.Distance, leftLaneNumber, Lanes, Config.SegmentLength * Config.TrackSize, Config.LaneOffset);

                    if (other.LaneNumber == leftLaneNumber)
                    {
                        if (math.abs(other.Distance - laneDistAfterChanging) < laneChangeClearance ||
                            math.abs(other.Distance + leftLane.LaneLength - laneDistAfterChanging) <
                            laneChangeClearance)
                        {
                            leftLaneOk =false;
                        }
                    }
                }

                if (rightLaneOk)
                {
                    var rightLaneNumber = car.LaneNumber - 1;
                    var rightLane = Lanes[rightLaneNumber];
                    var laneDistAfterChanging = TransformationUtils.GetDistanceOnLaneChange(car.LaneNumber,
                        car.Distance, rightLaneNumber, Lanes, Config.SegmentLength * Config.TrackSize, Config.LaneOffset);

                    if (other.LaneNumber == rightLaneNumber)
                    {
                        if (math.abs(other.Distance - laneDistAfterChanging) < laneChangeClearance ||
                            math.abs(other.Distance + rightLane.LaneLength - laneDistAfterChanging) <
                            laneChangeClearance)
                        {
                            rightLaneOk = false;
                        }
                    }
                }
            }

            // Give priority to merge back
            if (rightLaneOk)
            {

                car.Speed = car.DesiredSpeed;
                car.StartTransformation = TransformationUtils.GetWorldTransformation(car.Distance, Lanes[car.LaneNumber].LaneLength, Lanes[car.LaneNumber].LaneRadius, Config.SegmentLength * Config.TrackSize);

                var laneDistAfterChanging = TransformationUtils.GetDistanceOnLaneChange(car.LaneNumber,
                    car.Distance, car.LaneNumber - 1, Lanes, Config.SegmentLength * Config.TrackSize, Config.LaneOffset);

                car.LaneNumber--;
                car.Distance = laneDistAfterChanging;

                // Distance when car finishes transition
                float endDistance = (car.Distance + car.Speed * Config.LaneChangeTime) %
                                    Lanes[car.LaneNumber].LaneLength;
                car.EndTransformation = TransformationUtils.GetWorldTransformation(endDistance, Lanes[car.LaneNumber].LaneLength, Lanes[car.LaneNumber].LaneRadius, Config.SegmentLength * Config.TrackSize);
                car.LaneChangeProgress = 0.0f;
            }
            else if (distanceToFrontCar < (Config.FollowClearance + (car.Length + nearestFrontCar.Length) / 2))
            {
                if (leftLaneOk)
                {
                    car.Speed = car.DesiredSpeed * Config.MaxSpeedIncreaseWhilePassing;
                    car.StartTransformation = TransformationUtils.GetWorldTransformation(car.Distance, Lanes[car.LaneNumber].LaneLength, Lanes[car.LaneNumber].LaneRadius, Config.SegmentLength * Config.TrackSize);

                    var laneDistAfterChanging = TransformationUtils.GetDistanceOnLaneChange(car.LaneNumber,
                        car.Distance, car.LaneNumber + 1, Lanes, Config.SegmentLength * Config.TrackSize, Config.LaneOffset);

                    car.LaneNumber++;
                    car.Distance = laneDistAfterChanging;

                    float endDistance = (car.Distance + car.Speed * Config.LaneChangeTime) %
                                        Lanes[car.LaneNumber].LaneLength;
                    car.EndTransformation = TransformationUtils.GetWorldTransformation(endDistance, Lanes[car.LaneNumber].LaneLength, Lanes[car.LaneNumber].LaneRadius, Config.SegmentLength * Config.TrackSize);
                    car.LaneChangeProgress = 0.0f;
                }
                else
                {
                    car.Speed = nearestFrontCar.Speed;
                }
            }

            var lane = Lanes[car.LaneNumber];
            var laneRadius = lane.LaneRadius;
            var laneLength = lane.LaneLength;

            car.Distance += car.Speed * DeltaTime;
            if (car.Distance >= laneLength)
            {
                car.Distance -= laneLength;
            }

            float4x4 carTransform = float4x4.identity;
            if (car.LaneChangeProgress < 1.0)
            {
                // Interploate transformation and update progress
                carTransform = car.StartTransformation * (1.0f - car.LaneChangeProgress) +
                               car.EndTransformation * (car.LaneChangeProgress);
                car.LaneChangeProgress += DeltaTime / Config.LaneChangeTime;
            }
            else
            {
                carTransform = TransformationUtils.GetWorldTransformation(car.Distance, laneLength, laneRadius,
                    Config.SegmentLength * Config.TrackSize);
            }

            EntityParallelWriter.SetComponent(entityQueryIndex, car.Self, LocalTransform.FromMatrix(carTransform));
        }
    }
}
