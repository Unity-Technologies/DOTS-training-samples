using Authoring;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial struct WaterSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            int count = 0;
            foreach (var waterVolume
                     in SystemAPI.Query<RefRW<Volume>>()
                     .WithAll<WaterAuthoring.Water>())
            {
                waterVolume.ValueRW.value = config.maxCapacity;
                count++;
            }

            var buffer = SystemAPI.GetSingletonBuffer<ConfigAuthoring.WaterNode>();
            buffer.Length = count;
            int index = 0;
            foreach (var (transform, entity)
                     in SystemAPI.Query<RefRO<WorldTransform>>()
                     .WithAll<WaterAuthoring.Water>()
                     .WithEntityAccess())
            {
                buffer[index] = new ConfigAuthoring.WaterNode { Position = transform.ValueRO.Position, Node = entity };
                index++;
            }
        }
    }
}
