using Aspects;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            var dt = state.Time.DeltaTime;
            foreach (var car in SystemAPI.Query<CarAspect>())
            {
                car.T += dt;
                car.Position += car.Speed * car.Direction * dt;
            }
        }
    }
}
