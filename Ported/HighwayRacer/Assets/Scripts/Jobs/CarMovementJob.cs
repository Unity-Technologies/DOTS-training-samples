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
            Car neighbor = AllCars[0];//assume there is a car and that we will have one close to us
            float neighborDelta = 1000.0f;

            bool leftLaneOK = car.LaneNumber != Config.NumLanes - 1;//set to false if we are at the leftmost lane
            bool rightLaneOK = car.LaneNumber != 0;//set to false if we are at the rightmost lane

            foreach (var other in AllCars)
            {
                if (other.Index == car.Index) { continue; }

                float min = car.Distance - Config.LaneChangeClearance;
                float max = car.Distance + Config.LaneChangeClearance;

                int leftLane = car.LaneNumber + 1;
                int rightLane = car.LaneNumber - 1;

                if ((other.LaneNumber == leftLane || other.NewLaneNumber == leftLane) && other.Distance > min && other.Distance < max)
                {
                    leftLaneOK = false;
                }

                if ((other.LaneNumber == rightLane || other.NewLaneNumber == rightLane) && other.Distance > min && other.Distance < max)
                {
                    rightLaneOK = false;
                }

                if (other.LaneNumber != car.LaneNumber) { continue; }

                float delta = other.Distance - car.Distance;
                if (delta >= 0.0f && delta < neighborDelta)
                {
                    neighbor = other;
                    neighborDelta = delta;
                }
            }

            // If we're passing, our allowable speed changes from the cars desired speed to the percentage increase allowable while passing
            // var targetSpeed = car.IsPassing ? car.DesiredSpeed * config.MaxSpeedIncreaseWhilePassing : car.DesiredSpeed;
            var targetSpeed = car.DesiredSpeed;

            float laneRadius = 0.0f;
            float laneLength = 0.0f;
            // if (car.LaneChangeProgress >= 0.0f)
            if (car.NewLaneNumber >= 0)
            {
                targetSpeed = neighbor.Speed;
                //just worry about the transition
                car.LaneChangeProgress += DeltaTime;

                if (car.LaneChangeProgress > Config.LaneChangeTime)
                {
                    //finish it up
                    //keep our distance the same, relative to the new lane we are in
                    float percent = car.Distance / Lanes[car.LaneNumber].LaneLength;
                    car.Distance = percent * Lanes[car.NewLaneNumber].LaneLength;

                    car.LaneNumber = car.NewLaneNumber;
                    car.NewLaneNumber = -1;
                    car.LaneChangeProgress = -1.0f;


                    var lane = Lanes[car.LaneNumber];
                    laneRadius = lane.LaneRadius;
                    laneLength = lane.LaneLength;
                    car.IsPassing = false;
                }
                else
                {
                    //do some percentage between
                    float percent = car.LaneChangeProgress / Config.LaneChangeTime;
                    var oldLane = Lanes[car.LaneNumber];
                    var newLane = Lanes[car.NewLaneNumber];
                    laneRadius = math.lerp(oldLane.LaneRadius, newLane.LaneRadius, percent);
                    // laneLength = math.lerp(oldLane.LaneLength, newLane.LaneLength, percent);
                    laneLength = oldLane.LaneLength;//keeps us changing lane parallel, without speeding up due to length change
                }
            }
            else
            {
                // Give priority to merging back into the right lane
                if (rightLaneOK)
                {
                    car.LaneNumber--;
                    car.NewLaneNumber = car.LaneNumber - 1;
                    car.LaneChangeProgress = 0.0f;
                    car.IsPassing = false;
                }
                else if (neighborDelta < (Config.FollowClearance + (car.Length + neighbor.Length) / 2))
                {
                    // Otherwise if we're being blocked by a car in our lane, attempt to change left and pass
                    if (leftLaneOK)
                    {
                        // car.LaneNumber++;
                        car.NewLaneNumber = car.LaneNumber + 1;
                        car.LaneChangeProgress = 0.0f;
                        car.IsPassing = true;
                    }
                    // We can't pass, match our target speed to our neighbour
                    targetSpeed = neighbor.Speed;
                }


                var lane = Lanes[car.LaneNumber];
                laneRadius = lane.LaneRadius;
                laneLength = lane.LaneLength;
            }

            // If we're currently going less than our target, increase speed based on our acceleration and if we're going faster, decelerate
            // if (car.Speed < targetSpeed)
            // {
            //     car.Speed = math.min(targetSpeed, car.Speed + car.Acceleration);
            // }
            // else
            // {
            //     car.Speed = math.max(targetSpeed, car.Speed - car.Acceleration);
            // }
            car.Speed = targetSpeed;

            car.Distance += car.Speed * DeltaTime;
            if (car.Distance >= laneLength)
            {
                car.Distance -= laneLength;
            }

            float4x4 carTransform = TransformationUtils.GetWorldTransformation(car.Distance, laneLength, laneRadius, Config.SegmentLength * Config.TrackSize);
            EntityParallelWriter.SetComponent(entityQueryIndex, car.Self, LocalTransform.FromMatrix(carTransform));
        }
    }
}
