using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct FireGridHeatSetterSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
            state.RequireForUpdate<ConfigAuthoring.FlameHeat>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
            var heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();

            var index = 0;
            
            foreach (var (transform, color) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<FlameCell>())
            {
                var heat = heatMap[index];
                /*if (heat.Value != 0f && heat.Value != 1f)
                {
                    Debug.Log($"heat {heat.Value}");
                }*/
                transform.ValueRW.Position.y = heat.Value * 5;
                color.ValueRW.Value =
                    math.lerp(config.fireCoolColor, config.fireHotColor, heat.Value);
                index++;
            }
        }
    }
}
