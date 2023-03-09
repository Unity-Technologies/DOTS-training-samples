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
    [UpdateBefore(typeof(TransformSystemGroup))]
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

            foreach (var waterVolume
                     in SystemAPI.Query<RefRW<Volume>>()
                     .WithAll<WaterAuthoring.Water>())
            {
                waterVolume.ValueRW.value = config.maxCapacity;
            }
        }
    }
}
