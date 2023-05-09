using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Metro
{
    public partial struct PassengerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var em = state.EntityManager;

            var passengers = CollectionHelper.CreateNativeArray<Entity>(config.NumPassengers, Allocator.Temp);
            em.Instantiate(config.PassengerEntity, passengers);

            var random = new Random(1234);

            foreach ((var postTransformMatrix, var transform, var passenger) in
                     SystemAPI.Query<RefRW<PostTransformMatrix>, RefRW<LocalTransform>, RefRW<PassengerComponent>>()
                         .WithAll<PassengerComponent>())
            {
                var randomPos = random.NextFloat2Direction() * random.NextFloat(0.1f, 10f);
                var randomHeight = random.NextFloat(config.MinPassengerHeight, config.MaxPassengerHeight);
                postTransformMatrix.ValueRW.Value = float4x4.Scale(new float3(1f, randomHeight, 1f));
                transform.ValueRW.Position = new float3(randomPos.x, randomHeight * 0.5f, randomPos.y);
            }

            state.Enabled = false;
        }
    }
}