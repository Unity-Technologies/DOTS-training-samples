using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    partial struct WorldGenSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }
    }
}