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

            car.Speed = car.DesiredSpeed;
            car.Distance += car.Speed * DeltaTime;
            
            float3 moveAmount = new float3(car.Distance, 0, 0);
            var newPos = car.Position + moveAmount;
            car.Position = newPos;

        }
    }
}
