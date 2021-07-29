using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FireSpreadSystem : SystemBase
    {
        private double lastUpdateTime;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();
            RequireSingletonForUpdate<Temperature>();

        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();

            double elapsedTime = Time.ElapsedTime;

            if (lastUpdateTime + configValues.FireSimUpdateRate > elapsedTime)
            {
                return;
            }
            else
            {
                lastUpdateTime = elapsedTime;
            }

            var temperatureEntity = GetSingletonEntity<Temperature>();
            DynamicBuffer<Temperature> temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

            NativeArray<Temperature> temperatureArray = temperatureBuffer.AsNativeArray();

            NativeArray<Temperature> intermediateArray = new NativeArray<Temperature>(temperatureBuffer.Length, Allocator.TempJob);
            NativeArray<Temperature> spreadArray = new NativeArray<Temperature>(temperatureBuffer.Length, Allocator.TempJob);

            HorizontalSpreadJob horizontalSpreadJob = new HorizontalSpreadJob
            {
                Source = temperatureArray,
                Intermediate = intermediateArray,
                Spread = spreadArray,
                Columns = configValues.Columns,
                Rows = configValues.Rows,
                HeatRadius = configValues.HeatRadius,
                HeatTransferRate = configValues.HeatTransferRate * configValues.FireSimUpdateRate,
                Flashpoint = configValues.Flashpoint,
                MaxIntensity = 1.0f
            };

            Dependency = horizontalSpreadJob.Schedule(temperatureBuffer.Length, 32, Dependency);

            VerticalSpreadJob verticalSpreadJob = new VerticalSpreadJob
            {
                Intermediate = intermediateArray,                
                Spread = spreadArray,
                Destination = temperatureArray,
                Columns = configValues.Columns,
                Rows = configValues.Rows,
                HeatRadius = configValues.HeatRadius,
                HeatTransferRate = configValues.HeatTransferRate * configValues.FireSimUpdateRate,
                Flashpoint = configValues.Flashpoint,
                MaxIntensity = 1.0f
            };

            Dependency = verticalSpreadJob.Schedule(temperatureBuffer.Length, 32, Dependency);

            intermediateArray.Dispose(Dependency);
            spreadArray.Dispose(Dependency);
        }
    }

    struct HorizontalSpreadJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Temperature> Source;

        public NativeArray<Temperature> Intermediate;
        public NativeArray<Temperature> Spread;

        public int Columns;
        public int Rows;
        public int HeatRadius;
        public float HeatTransferRate;
        public float Flashpoint;
        public float MaxIntensity;

        public void Execute(int index)
        {
            int column = index % Columns;
            int row = index / Columns;

            float temperature = Source[column + row * Columns].Intensity;
            float spreadTemperature = 0;

            for (int spread = -HeatRadius; spread <= HeatRadius; ++spread)
            {
                if (spread != 0 &&
                    spread + column >= 0 &&
                    spread + column < Columns)
                {
                    float sourceTemperature = Source[column + spread + row * Columns].Intensity;

                    if (sourceTemperature > Flashpoint)
                    {
                        float distanceFalloff = math.rsqrt(math.max(1.0f, math.abs(spread)));
                        spreadTemperature += sourceTemperature * HeatTransferRate * distanceFalloff;
                    }
                }
            }

            Spread[column + row * Columns] = new Temperature { Intensity = spreadTemperature};
            Intermediate[column + row * Columns] = new Temperature { Intensity = temperature };
        }
    }

    struct VerticalSpreadJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Temperature> Intermediate;
        [ReadOnly]
        public NativeArray<Temperature> Spread;

        public NativeArray<Temperature> Destination;
        public int Columns;
        public int Rows;
        public int HeatRadius;
        public float HeatTransferRate;
        public float Flashpoint;
        public float MaxIntensity;

        public void Execute(int index)
        {
            int column = index % Columns;
            int row = index / Columns;

            float temperature = Intermediate[column + row * Columns].Intensity;

            for (int spread = -HeatRadius; spread <= HeatRadius; ++spread)
            {
                if (spread + row >= 0 &&
                    spread + row < Rows)
                {
                    if (spread != 0)
                    {
                        float sourceTemperature = Intermediate[column + (row + spread) * Columns].Intensity;

                        if (sourceTemperature > Flashpoint)
                        {
                            float distanceFalloff = math.rsqrt(math.max(1.0f, math.abs(spread)));
                            temperature += sourceTemperature * HeatTransferRate * distanceFalloff;
                        }
                    }

                    temperature += Spread[column + (row + spread) * Columns].Intensity;
                }
            }

            if (temperature > MaxIntensity)
                temperature = MaxIntensity;

            Destination[column + row * Columns] = new Temperature { Intensity = temperature };
        }
    }
}
