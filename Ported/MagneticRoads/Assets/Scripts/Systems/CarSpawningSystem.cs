using Aspects;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    partial struct CarSpawningSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // This system should not run before the Config singleton has been loaded.
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var cars = CollectionHelper.CreateNativeArray<Entity>(config.CarCount, Allocator.Temp);

            foreach (var car in cars )
            {
                ecb.SetComponentEnabled<WaitingAtIntersection>(car, false);
            }

            ecb.Instantiate(config.CarPrefab, cars);
            
            var tempEntity = ecb.CreateEntity();
            ecb.AddComponent<Lane>(tempEntity);
            var buffer = ecb.AddBuffer<CarDynamicBuffer>(tempEntity).Reinterpret<Entity>();

            buffer.AddRange(cars);

            var nonLaneCars = CollectionHelper.CreateNativeArray<Entity>(100, Allocator.Temp);
            ecb.Instantiate(config.CarPrefab, nonLaneCars);

            state.Enabled = false;
        }
    }
}
