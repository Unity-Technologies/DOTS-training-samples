using Aspects;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jobs
{
    
    [BurstCompile]
    public partial struct CarMovementJob : IJobEntity
    {
        [ReadOnly]
        public int frameCount;

        [ReadOnly]
        public float DeltaTime;

        [ReadOnly]
        public Config config;

        [BurstCompile]
        private void Execute(ref CarAspect car)
        {
            float desiredSpeed = car.CruisingSpeed;
            if (car.Lane < car.DesiredLane)
            {
                car.Lane = math.min(car.Lane + config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.Lane > car.DesiredLane)
            {
                car.Lane = math.max(car.Lane - config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.OvertakeModeCountdown > 0)
            {
                // in overtake mode, move faster and tick down the timer
                desiredSpeed = car.OvertakeSpeed;
                car.OvertakeModeCountdown = math.max(car.OvertakeModeCountdown - DeltaTime, 0);
            }
            else if (car.TEMP_NextLaneChangeCountdown <= 0) // regular cruising mode, handle lane changes
            {
                //randomly change lanes
                if (frameCount % 2 == 1)
                    car.DesiredLane = math.min(car.Lane + 1, config.NumLanes - 1);
                else
                    car.DesiredLane = math.max(car.Lane - 1, 0);
                car.TEMP_NextLaneChangeCountdown = 3;
                car.OvertakeModeCountdown = config.OvertakeMaxDuration;
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
            car.Position = new float3(car.Distance, 0, car.Lane);
        }
    }
}
