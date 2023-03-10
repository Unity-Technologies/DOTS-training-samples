using Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;
using UnityEngine;
using System;

namespace Jobs
{
    [WithAll(typeof(CarData))]
    [BurstCompile]
    public partial struct CarMovementJob : IJobEntity
    {
        [ReadOnly]
        public int frameCount;

        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        public Config config;

        [ReadOnly]
        public NativeArray<CarData> allCars;

       // [ReadOnly]
        //public NativeArray<LocalTransform> allCarTransforms;

        [BurstCompile]
        private void Execute(ref CarAspect car)
        {
            CarData other;
            CarData nearestFrontCar = default;
            bool foundFrontCar = false;

            float distanceToFrontCar = float.MaxValue;
            float desiredSpeed = car.CruisingSpeed;

            // while changing lane, don't do anything else
            if (car.CurrentLane < car.DesiredLane)
            {
                car.CurrentLane = math.min(car.CurrentLane + config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.CurrentLane > car.DesiredLane)
            {
                car.CurrentLane = math.max(car.CurrentLane - config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.OvertakeModeCountdown > 0)
            {
                // in overtake mode, move faster and tick down the timer
                desiredSpeed = car.OvertakeSpeed;
                car.OvertakeModeCountdown = math.max(car.OvertakeModeCountdown - DeltaTime, 0);
                if (car.OvertakeModeCountdown == 0)
                {
                    car.DesiredLane = car.OvertakeModeReturnToLane;
                }
            }
            else if (car.TEMP_NextLaneChangeCountdown <= 0)
            {
                // in regular cruising mode, randomly change lanes
                if (frameCount % 2 == 1)
                    car.DesiredLane = math.min(car.CurrentLane + 1, config.NumLanes - 1);
                else
                    car.DesiredLane = math.max(car.CurrentLane - 1, 0);

                if (car.DesiredLane != car.CurrentLane)
                {
                    car.OvertakeModeCountdown = config.OvertakeMaxDuration;
                    car.OvertakeModeReturnToLane = car.CurrentLane;
                }
                car.TEMP_NextLaneChangeCountdown = 3;
            }
            else
            {
                car.TEMP_NextLaneChangeCountdown -= DeltaTime;
            }
            int start = Math.Max(0, car.DistanceIndexCache-3); // Check 3 cars overall before the on last frame
            for (int i = start; i < allCars.Length+10; i++) // we iterate more to loop, in case you are the last one following the third
            {
                int realIndex = i % allCars.Length;
                other = allCars[realIndex];

                var distToOtherCarInLane = other.Distance - car.Distance;

                if (distToOtherCarInLane > 0.0f)    // so in front
                {
                    car.DistanceIndexCache = realIndex; // last checked car
                    if (car.CurrentLane == other.CurrentLane /*&& other.DesiredLane == other.CurrentLane*/)
                    {
                        if (distToOtherCarInLane < distanceToFrontCar)
                        {
                            distanceToFrontCar = distToOtherCarInLane;
                            nearestFrontCar = other;
                            foundFrontCar = true;
                        }
                    }
                }
                else
                {
                    break; // we overpass car in the same lane
                }
            }
            float minDistance = nearestFrontCar.Distance - car.Distance;

            if (foundFrontCar && minDistance < car.Length)
            {
                if (minDistance > 0.0f)
                    desiredSpeed = nearestFrontCar.Speed;
                else
                    desiredSpeed = 0.0f;
            }

            if (car.Speed < desiredSpeed)
                car.Speed = math.min(car.Speed + car.Acceleration, desiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, desiredSpeed);

            if (desiredSpeed > car.OvertakeSpeed)
            {
                car.DesiredSpeed = 1.0f;
            }
            else
            {
                if (desiredSpeed == car.CruisingSpeed)
                    car.DesiredSpeed = 0.0f;
                else
                    car.DesiredSpeed = -1.0f;
                
            }

            car.Distance = (car.Distance + car.Speed * DeltaTime) % config.HighwayMaxSize;
            car.Position = new float3(car.Distance, 0, car.CurrentLane);
        }
    }
}
