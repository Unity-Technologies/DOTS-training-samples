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
        // DW: This is beyond greasy, but we can't have native arrays of native arrays in jobs, so just forcing it like
        // this for now since we have a static number of segments
        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment0;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment1;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment2;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment3;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment4;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment5;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment6;

        [ReadOnly]
        public NativeArray<Car> AllCarsInSegment7;

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
            // Check only cars in the same segment as we are to cut down on iterations.
            // TODO: Nice to have would be also check the segment in front or behind if we're close to the border
            NativeArray<Car> carsInSegment = AllCarsInSegment0;

            // Greasy, but it's late and we dont' have time to figure out a nice solution here
            switch (car.SegmentNumber)
            {
                case 1:
                    carsInSegment = AllCarsInSegment1;
                    break;
                case 2:
                    carsInSegment = AllCarsInSegment2;
                    break;
                case 3:
                    carsInSegment = AllCarsInSegment3;
                    break;
                case 4:
                    carsInSegment = AllCarsInSegment4;
                    break;
                case 5:
                    carsInSegment = AllCarsInSegment5;
                    break;
                case 6:
                    carsInSegment = AllCarsInSegment6;
                    break;
                case 7:
                    carsInSegment = AllCarsInSegment7;
                    break;
            }


            float distanceToFrontCar = float.MaxValue;
            Car nearestFrontCar = AllCarsInSegment0[0];//assume there is a car and that we will have one close to us

            bool leftLaneOk = car.LaneNumber != Config.NumLanes - 1;//set to false if we are at the leftmost lane
            bool rightLaneOk = car.LaneNumber != 0;//set to false if we are at the rightmost lane

            // Don't change lane if in lane changing
            leftLaneOk = leftLaneOk && car.LaneChangeProgress > 1.0f;
            rightLaneOk = rightLaneOk && car.LaneChangeProgress > 1.0f;

            foreach (var other in carsInSegment)
            {

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

            // Update our current segment if it has changed
            var newSegment = TransformationUtils.GetSegmentIndexFromDistance(car.Distance, laneLength,
                Config.SegmentLength * Config.TrackSize);
            if (car.SegmentNumber != newSegment)
            {
                var carData = car.Car.ValueRW;
                carData.SegmentNumber = newSegment;
                EntityParallelWriter.SetComponent(entityQueryIndex, car.Self, carData);
                EntityParallelWriter.SetSharedComponent(entityQueryIndex, car.Self, new SegmentNumber { SegmentId = newSegment });
            }
        }
    }
}
