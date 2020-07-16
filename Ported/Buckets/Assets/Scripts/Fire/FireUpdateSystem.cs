using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Fire
{
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

                    position.Value.y = startHeight.Value + bounds.SizeY / 2f * temperature.Value;

                }).ScheduleParallel();
        }

        /// <summary>
        ///   <para>Compares two floating point values and returns true if they are similar.</para>
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
