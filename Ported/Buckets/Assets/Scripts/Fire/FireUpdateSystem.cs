using System;
using System.Data.SqlTypes;
using Unity.Collections;
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

        protected override void OnStartRunning()
        {
            base.OnStartRunning();

        }

        protected override void OnStopRunning()
        {

            base.OnStopRunning();

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

            var fireBufferEntity = GetSingletonEntity<FireBuffer>();
            var gridBufferLookup = GetBufferFromEntity<FireBufferElement>();
            var gridBuffer = gridBufferLookup[fireBufferEntity];

            var gridArray = gridBuffer.AsNativeArray();
            var gridMetaData = EntityManager.GetComponentData<FireBufferMetaData>(fireBufferEntity);

            // Create cached native array of temperature components to read neighbor temperature data in jobs
            NativeArray<TemperatureComponent> temperatureComponents = new NativeArray<TemperatureComponent>(gridArray.Length, Allocator.TempJob);
            for (int i = 0; i < gridArray.Length; i++)
            {
                temperatureComponents[i] = EntityManager.GetComponentData<TemperatureComponent>(gridArray[i].FireEntity);
            }

            // Update fires in scene
            Entities
                .WithDeallocateOnJobCompletion(temperatureComponents)
                .ForEach((Entity fireEntity, int entityInQueryIndex, ref Translation position, ref TemperatureComponent temperature, ref FireColor color,
                    in BoundsComponent bounds, in StartHeight startHeight, in FireColorPalette pallete) =>
                {
                    var temp = math.clamp(temperature.Value, 0, 1);

                    // If temp is 0, velocity is put out
                    bool fireOut = UnityMathUtils.Approximately(temp, 0f, epsilon);
                    if (fireOut)
                    {
                        temperature.Velocity = 0;

                        // Find neighboring temperatures
                        var neighbors = FireSearch.GetNeighboringIndicies(temperature.GridIndex, gridMetaData.CountX,gridMetaData.CountZ);

                        var topTemp = (neighbors.Top == -1) ? 0 : temperatureComponents[neighbors.Top].Value;
                        var bottomTemp = (neighbors.Bottom == -1) ? 0 : temperatureComponents[neighbors.Bottom].Value;
                        var leftTemp = (neighbors.Left == -1) ? 0 : temperatureComponents[neighbors.Left].Value;
                        var rightTemp = (neighbors.Right == -1) ? 0 : temperatureComponents[neighbors.Right].Value;

                        // Find max temp of neighbors
                        var maxTemp = math.max(topTemp, bottomTemp);
                        maxTemp = math.max(maxTemp, leftTemp);
                        maxTemp = math.max(maxTemp, rightTemp);

                        // TODO Use authoring spread flashpoint + Use neighboring velocity instead of fixed val
                        if (maxTemp >= 0.95f)
                        {
                            temperature.Velocity = 0.05f;
                        }
                    }

                    // Update temp with velocity
                    var deltaVel = temperature.Velocity * deltaTime;
                    temperature.Value = math.clamp(temp + deltaVel, 0, 1);

                    // Compute variance for fire height fluctation
                    float fireVariance = math.sin( 5 * temperature.Value * elapsedTime + 100 * math.PI * startHeight.Variance) * startHeight.Variance * temperature.Value;

                    // Compute new height
                    float newHeight = bounds.SizeY / 2f + fireVariance;
                    position.Value.y = startHeight.Value + newHeight * temperature.Value;

                    // Update fire color
                    color.Value = fireOut ? pallete.UnlitColor : UnityMathUtils.Lerp(pallete.LitLowColor, pallete.LitHighColor, temperature.Value);
                }).ScheduleParallel();
        }
    }
}
