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
            var track = ecb.CreateEntity();
            
            ecb.AddComponent<Track>(track);
            ecb.SetComponent(track, new Track
            {
                StartPos = new float3(0,0,0),
                StartNorm = new float3(0,1,0),
                StartTang = new float3(0,0,1),
                EndPos = new float3(0,0,10),
                EndNorm = new float3(0,1,0),
                EndTang = new float3(0,0,1),
            });
            ecb.SetComponent(car, new Car { Track = track });
            state.Enabled = false;
        }
    }
}