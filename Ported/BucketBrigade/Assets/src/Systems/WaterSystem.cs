using System;
using src.Components;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using Unity.Jobs;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class WaterReplenishSystem: SystemBase
    {
        private double lastUpdateTime;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();

            double elapsedTime = Time.ElapsedTime;
            double deltaTime = elapsedTime - lastUpdateTime;
            if (lastUpdateTime + configValues.WaterSourceUpdateTime > elapsedTime)
            {
                return;
            }
            else
            {
                lastUpdateTime = elapsedTime;
            }

            Entities
                .WithName("UpdateWaterSources")
                .WithBurst()
                .ForEach((int entityInQueryIndex, ref LocalToParent localToParent, ref WaterTag waterSource) =>
                {
                    waterSource.CurrentWaterVolume = Math.Min((float)(waterSource.CurrentWaterVolume + configValues.WaterSourceReplenishRate * deltaTime), waterSource.MaximumWaterVolume);

                    var position = localToParent.Position;
                    float scale = UnityEngine.Mathf.Clamp(waterSource.CurrentWaterVolume / waterSource.MaximumWaterVolume, 0f, 1f);
                    localToParent.Value = float4x4.TRS(position, quaternion.identity,  new float3(scale, scale, scale));
                }).ScheduleParallel();
        }
    }
}
