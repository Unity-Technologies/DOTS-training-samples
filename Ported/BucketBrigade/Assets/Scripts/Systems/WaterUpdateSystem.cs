using Components;
using System;
using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct WaterUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
            state.RequireForUpdate<WaterAuthoring.Water>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            foreach (var (transform, volume) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<Volume>>()
                     .WithAll<WaterAuthoring.Water>())
            {
                volume.ValueRW.value = math.clamp(volume.ValueRO.value + config.refillRate, 0, config.maxCapacity);
                transform.ValueRW.Scale = math.lerp(0, 1, volume.ValueRO.value / config.maxCapacity);
            }
        }
    }
}
