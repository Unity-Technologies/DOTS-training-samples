using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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

            Random random = new Random(123);
            foreach (var (transform, color) in 
                SystemAPI.Query<RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<FlameCell>())
            {
                
                //setting colour based on heat
                var heat = heatMap[index];
                /*if (heat.Value != 0f && heat.Value != 1f)
                {
                    Debug.Log($"heat {heat.Value}");
                }*/

                if (heat.Value < config.flashpoint)
                {
                    transform.ValueRW.Position.y = -(config.maxFlameHeight * 0.5f) + random.NextFloat(0.01f,0.02f);
                    color.ValueRW.Value = config.fireNeutralColor;
                }
                else
                {
                    float noiseValue = noise.cnoise(new float2(Time.time - index * config.flickerRate - heat.Value, heat.Value));
                    transform.ValueRW.Position.y = -(config.maxFlameHeight * 0.5f) + heat.Value + noiseValue;
                    color.ValueRW.Value =
                        math.lerp(config.fireCoolColor, config.fireHotColor, heat.Value);
                }
                index++;
                
                
            }
        }
    }
}
