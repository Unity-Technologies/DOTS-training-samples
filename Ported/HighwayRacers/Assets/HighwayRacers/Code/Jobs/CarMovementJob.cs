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
            if(car.Speed < car.DesiredSpeed)
                car.Speed = math.min(car.Speed+ car.Acceleration, car.DesiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, car.DesiredSpeed);

            if (car.Lane < car.DesiredLane)
            {
                car.Lane = math.min(car.Lane + config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.Lane > car.DesiredLane)
            {
                car.Lane = math.max(car.Lane - config.SwitchLanesSpeed * DeltaTime, car.DesiredLane);
            }
            else if (car.TEMP_NextLaneChangeCountdown <= 0)
            {
                //randomly change lanes
                if (frameCount % 2 == 1)
                    car.DesiredLane = math.min(car.Lane + 1, config.NumLanes - 1);
                else
                    car.DesiredLane = math.max(car.Lane - 1, 0);
                car.TEMP_NextLaneChangeCountdown = 3;
            }
            else
            {
                car.TEMP_NextLaneChangeCountdown -= DeltaTime;
            }

            car.Distance += car.Speed * DeltaTime;
            car.Position = new float3(car.Distance, 0, car.Lane);
        }
    }
}
