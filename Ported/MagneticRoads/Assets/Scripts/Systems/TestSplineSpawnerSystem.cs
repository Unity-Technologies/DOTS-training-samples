using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Util;

namespace Systems
{
    [BurstCompile]
    partial struct TestSplineSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // This system should not run before the Config singleton has been loaded.
            state.RequireForUpdate<TestSplineConfig>();
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

            var random = Random.CreateFromIndex(1234);
            var hue = random.NextFloat();
            
            URPMaterialPropertyBaseColor RandomColor()
            {
                // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
                hue = (hue + 0.618034005f) % 1;
                var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
                return new URPMaterialPropertyBaseColor {Value = (UnityEngine.Vector4) color};
            }
            
            var car = ecb.Instantiate(config.CarPrefab);
            var car2 = ecb.Instantiate(config.CarPrefab);
            var car3 = ecb.Instantiate(config.CarPrefab);
            var car4 = ecb.Instantiate(config.CarPrefab);
            var car5 = ecb.Instantiate(config.CarPrefab);
            var rs = ecb.CreateEntity();
            
            ecb.AddComponent<RoadSegment>(rs);
            var Start = new Spline.RoadTerminator
            {
                Position = new float3(0, 0, 0),
                Normal = new float3(0, 1, 0),
                Tangent = new float3(0, 0, 1)
            };
            var End = new Spline.RoadTerminator
            {
                Position = new float3(4, 0, 4),
                Normal = new float3(0, 1, 0),
                Tangent = new float3(1, 0, 0),
            };
            ecb.SetComponent(rs, new RoadSegment
            {
                Start = Start,
                End = End,
                Length = Spline.EvaluateLength(Start, End)
            });
            
            ecb.SetComponent(car, new Car { RoadSegment = rs, Speed = 3f, LaneNumber = 1});
            ecb.AddComponent(car, RandomColor() );
            ecb.SetComponent(car2, new Car{ RoadSegment = rs, Speed = 3f, LaneNumber = 2});
            ecb.AddComponent(car2, RandomColor() );
            ecb.SetComponent(car3, new Car{ RoadSegment = rs, Speed = 3f, LaneNumber = 3});
            ecb.AddComponent(car3, RandomColor() );
            ecb.SetComponent(car4, new Car{ RoadSegment = rs, Speed = 3f, LaneNumber = 4});
            ecb.AddComponent(car4, RandomColor() );
            ecb.SetComponent(car5, new Car{ RoadSegment = rs, Speed = 3f, LaneNumber = 1, T = 0.5f});
            ecb.AddComponent(car5, RandomColor() );
            
            var lane1 = ecb.CreateEntity();
            ecb.AddComponent<Lane>(lane1);
            var lane2 = ecb.CreateEntity();
            ecb.AddComponent<Lane>(lane2);
            var lane3 = ecb.CreateEntity();
            ecb.AddComponent<Lane>(lane3);
            var lane4 = ecb.CreateEntity();
            ecb.AddComponent<Lane>(lane4);
            var lane1Buffer = ecb.AddBuffer<CarDynamicBuffer>(lane1).Reinterpret<Entity>();
            var lane2Buffer = ecb.AddBuffer<CarDynamicBuffer>(lane2).Reinterpret<Entity>();
            var lane3Buffer = ecb.AddBuffer<CarDynamicBuffer>(lane3).Reinterpret<Entity>();
            var lane4Buffer = ecb.AddBuffer<CarDynamicBuffer>(lane4).Reinterpret<Entity>();
            
            lane1Buffer.Add(car);
            lane2Buffer.Add(car2);
            lane3Buffer.Add(car3);
            lane4Buffer.Add(car4);
            lane1Buffer.Add(car5);
            
            state.Enabled = false;
        }
    }
}
