using System;
using System.Data.SqlTypes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Fire
{
    [GenerateAuthoringComponent]
    public struct FireSpreadProperties : IComponentData
    {

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
                .ForEach((Entity fireEntity, int entityInQueryIndex, ref Translation position, ref TemperatureComponent temperature, in BoundsComponent bounds, in StartHeight startHeight) =>
                {
                    var temp = math.clamp(temperature.Value, 0, 1);

                    // If temp is 0, velocity is put out
                    if (Approximately(temp, 0f, epsilon))
                    {
                        temperature.Value = 0;
                    }

                    var deltaVel = temperature.Velocity * deltaTime;
                    temperature.Value = temp + deltaVel;

                    float fireVariance = math.sin( 5 * temperature.Value * elapsedTime + 100 * math.PI * startHeight.Variance) * startHeight.Variance * temperature.Value;

                    float newHeight = bounds.SizeY / 2f + fireVariance;
                    position.Value.y = startHeight.Value + newHeight * temperature.Value;

                }).ScheduleParallel();
        }

        /// <summary>
        /// Compares two floating point values and returns true if they are similar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        public static bool Approximately(float a, float b, float epsilon)
        {
            return (double)math.abs(b - a) < (double)math.max(1E-06f * math.max(math.abs(a), math.abs(b)), epsilon * 8f);
        }
    }
}
