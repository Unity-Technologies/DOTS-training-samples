using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Metro
{
    public partial struct PassengerMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var em = state.EntityManager;
            Debug.Log("Moving");
            foreach (var transform in
                     SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<PassengerComponent>())
            {
                var position = transform.ValueRO.Position;
                position.x += 0.01f;
                transform.ValueRW.Position = position;
            }
            
        }
    }
}
