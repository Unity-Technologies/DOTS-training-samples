using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct CarTravelSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                car.Position += new float3(0f, 0f, 1f) * car.Speed * car.Direction;
            }
        }
    }
}
