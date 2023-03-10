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

        //[ReadOnly]
        //public NativeArray<CarPosition> allCars;

        //[ReadOnly]
        //public NativeArray<LocalTransform> allCarTransforms;

        [BurstCompile]
        private void Execute(ref CarPosition carPos, ref CarData carData, ref CarParameters carParameters, ref LaneChangeState laneChange, ref LocalTransform transform)
        {
            laneChange.requestChangeUp = false;
            laneChange.requestChangeDown = false;

            float desiredSpeed = carParameters.CruisingSpeed;
            if (laneChange.distFromCarInFront < 10.0f)
            {
                float tailgatingSpeed = (laneChange.distFromCarInFront - config.CarLength)*10;
                if (tailgatingSpeed < desiredSpeed)
                {
                    desiredSpeed = tailgatingSpeed;
                    carData.OvertakeModeCountdown = 0;
                }
            }

            // while changing lane, don't do anything else
            if (carPos.CurrentLane < carData.DesiredLane)
            {
                carPos.CurrentLane = math.min(carPos.CurrentLane + config.SwitchLanesSpeed * DeltaTime, carData.DesiredLane);
            }
            else if (carPos.CurrentLane > carData.DesiredLane)
            {
                carPos.CurrentLane = math.max(carPos.CurrentLane - config.SwitchLanesSpeed * DeltaTime, carData.DesiredLane);
            }
            else
            {
                if (laneChange.approveChangeUp)
                {
                    carData.DesiredLane = carPos.CurrentLane + 1;

                    if (carData.OvertakeModeCountdown > 0)
                        carData.OvertakeModeCountdown = 0; // if I was overtaking, this lane change is a merge back: stop overtaking
                    else
                        carData.OvertakeModeCountdown = config.OvertakeMaxDuration; // if I wasn't overtaking, this lane change is an overtake
                }
                else if (laneChange.approveChangeDown)
                {
                    carData.DesiredLane = carPos.CurrentLane - 1;

                    if (carData.OvertakeModeCountdown > 0)
                        carData.OvertakeModeCountdown = 0;
                    else
                        carData.OvertakeModeCountdown = config.OvertakeMaxDuration;
                }
                else if (desiredSpeed < carParameters.CruisingSpeed)
                {
                    // if I'm being slowed down, try to change lanes
                    laneChange.requestChangeUp = (carPos.CurrentLane < config.NumLanes);
                    laneChange.requestChangeDown = (carPos.CurrentLane > 0);
                    carData.OvertakeModeCountdown = 0;
                }
                else if (carData.OvertakeModeCountdown > 0)
                {
                    // in overtake mode, move faster and tick down the timer
                    if (desiredSpeed == carParameters.CruisingSpeed)
                        desiredSpeed = carParameters.OvertakeSpeed;

                    // in the last second of overtake mode, try to merge back
                    if (carData.OvertakeModeCountdown < 1.0f)
                    {
                        if (carPos.CurrentLane > carData.OvertakeModeReturnToLane)
                            laneChange.requestChangeDown = true;
                        else
                            laneChange.requestChangeUp = true;
                    }

                    carData.OvertakeModeCountdown = math.max(carData.OvertakeModeCountdown - DeltaTime, 0);
                }
            }

            if (carPos.Speed < desiredSpeed)
                carPos.Speed = math.min(carPos.Speed + carData.Acceleration, desiredSpeed);
            else
                carPos.Speed = math.max(carPos.Speed - carData.Acceleration, desiredSpeed);

            carPos.Distance = (carPos.Distance + carPos.Speed * DeltaTime) % config.HighwayMaxSize;
            transform.Position = new float3(carPos.Distance, 0, carPos.CurrentLane);

            laneChange.approveChangeUp = false;
            laneChange.approveChangeDown = false;
        }
    }
}
