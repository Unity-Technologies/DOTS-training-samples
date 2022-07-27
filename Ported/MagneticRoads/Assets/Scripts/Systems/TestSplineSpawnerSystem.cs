using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Util;

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
                Start = new Spline.RoadTerminator
                {
                    Position = new float3(0,0,0),
                    Normal = new float3(0,1,0),
                    Tangent = new float3(0,0,1)
                },
                End = new Spline.RoadTerminator
                {
                    Position = new float3(0,0,100),
                    Normal = new float3(0,1,0),
                    Tangent = new float3(0,0,1)
                    
                }
            });
            
            ecb.SetComponent(car2, new Car{ RoadSegment = rs, T = 0.2f});
            ecb.SetComponent(car, new Car { RoadSegment = rs });
            
            var tempEntity = ecb.CreateEntity();
            ecb.AddComponent<Lane>(tempEntity);
            var buffer = ecb.AddBuffer<CarDynamicBuffer>(tempEntity).Reinterpret<Entity>();

            buffer.Add(car);
            buffer.Add(car2);
            
            state.Enabled = false;
        }
    }
}