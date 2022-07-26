using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [BurstCompile]
    partial struct TestSplineSpawnerSystem : ISystem
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
            var config = SystemAPI.GetSingleton<TestSplineConfig>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var car = ecb.Instantiate(config.CarPrefab);
            var car2 = ecb.Instantiate(config.CarPrefab);
            var rs = ecb.CreateEntity();
            
            ecb.AddComponent<RoadSegment>(rs);
            ecb.SetComponent(rs, new RoadSegment
            {
                StartPos = new float3(0,0,0),
                StartNorm = new float3(0,1,0),
                StartTang = new float3(0,0,1),
                EndPos = new float3(0,0,100),
                EndNorm = new float3(0,1,0),
                EndTang = new float3(0,0,1),
            });
            
            ecb.SetComponent(car2, new Car{ RoadSegment = rs, T = 0.5f});
            ecb.SetComponent(car, new Car { RoadSegment = rs });
            state.Enabled = false;
        }
    }
}