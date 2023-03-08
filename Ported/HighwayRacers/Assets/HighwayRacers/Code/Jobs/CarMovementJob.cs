using Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;
using UnityEngine;

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

        [ReadOnly]
        public NativeArray<LocalTransform> allCarTransforms;

        [BurstCompile]
        private void Execute(ref CarAspect car)
        {
            CarData other;
            CarData nearestFrontCar = default;

            float distanceToFrontCar = float.MaxValue;

            if (car.Speed < car.DesiredSpeed)
                car.Speed = math.min(car.Speed+ car.Acceleration, car.DesiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, car.DesiredSpeed);

            for (int i = 0; i < allCars.Length; i++)
            {
               other = allCars[i];
               if (other.CurrentLane == car.CurrentLane)
               {
                    var distToOtherCarInLane = Vector3.Distance(allCarTransforms[i].Position, car.Position);

                    if (distToOtherCarInLane < distanceToFrontCar)
                    {
                        distanceToFrontCar = distToOtherCarInLane;
                        nearestFrontCar = other;
                    }
                 }
             }

            float desiredSpeed = car.CruisingSpeed;
            if (distanceToFrontCar < (100.0f + (car.Length + nearestFrontCar.Length) / 2))
            {
                desiredSpeed = nearestFrontCar.Speed - 2.0f;
            }


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

            if (car.Speed < desiredSpeed)
                car.Speed = math.min(car.Speed + car.Acceleration, desiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, desiredSpeed);
            
            car.Distance = (car.Distance + car.Speed * DeltaTime) % config.HighwayMaxSize;
            car.Position = new float3(car.Distance, 0, car.CurrentLane);
        }
    }
}
