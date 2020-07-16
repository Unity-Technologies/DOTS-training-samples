using System;
using System.Data.SqlTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;

namespace Fire
{
    [GenerateAuthoringComponent]
    public struct FireSpreadProperties : IComponentData
    {
        public float SpreadFlashPoint;
    }

    public class FireUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            // Check if we have initialized
            EntityQuery queryGroup = GetEntityQuery(typeof(Initialized));
            if (queryGroup.CalculateEntityCount() == 0)
            {
                return;
            }

            float deltaTime = Time.DeltaTime;
            float epsilon = Mathf.Epsilon;
            float elapsedTime = (float)Time.ElapsedTime;

            // Check current state to spread fires

            // Update fires in scene
            Entities
                .ForEach((Entity fireEntity, int entityInQueryIndex, ref Translation position, ref TemperatureComponent temperature, ref FireColor color,
                    in BoundsComponent bounds, in StartHeight startHeight, in FireColorPalette pallete) =>
                {
                    var temp = math.clamp(temperature.Value, 0, 1);

                    // If temp is 0, velocity is put out
                    bool fireOut = UnityMathUtils.Approximately(temp, 0f, epsilon);
                    if (fireOut)
                    {
                        temperature.Velocity = 0;
                    }

                    var deltaVel = temperature.Velocity * deltaTime;
                    temperature.Value = math.clamp(temp + deltaVel, 0, 1);

                    float fireVariance = math.sin( 5 * temperature.Value * elapsedTime + 100 * math.PI * startHeight.Variance) * startHeight.Variance * temperature.Value;

                    float newHeight = bounds.SizeY / 2f + fireVariance;
                    position.Value.y = startHeight.Value + newHeight * temperature.Value;

                    if (fireOut)
                    {
                        color.Value = pallete.UnlitColor;
                    }
                    else
                    {
                        color.Value = UnityMathUtils.Lerp(pallete.LitLowColor, pallete.LitHighColor, temperature.Value);
                    }

                }).ScheduleParallel();
        }

  
    }
}
