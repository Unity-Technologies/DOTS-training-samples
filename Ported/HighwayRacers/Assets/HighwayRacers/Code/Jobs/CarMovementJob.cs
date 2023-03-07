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
        public float DeltaTime;

        [BurstCompile]
        private void Execute(ref CarAspect car)
        {
            if(car.Speed < car.DesiredSpeed)
                car.Speed = math.min(car.Speed+ car.Acceleration, car.DesiredSpeed);
            else
                car.Speed = math.max(car.Speed - car.Acceleration, car.DesiredSpeed);

            car.Distance += car.Speed * DeltaTime;
            car.Position = new float3(car.Distance, 0, car.Lane);
        }
    }
}
