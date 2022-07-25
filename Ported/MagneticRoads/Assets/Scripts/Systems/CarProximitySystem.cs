using Aspects;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct CarProximitySystem : ISystem
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
                // Create a partial radius pointing forwards from the car
                // See if any cars are there and if there are then slow the
                
            }
        }
    }
}
