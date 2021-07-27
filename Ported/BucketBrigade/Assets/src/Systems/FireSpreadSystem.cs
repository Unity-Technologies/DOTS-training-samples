using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

namespace src.Systems
{    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FireSpreadSystem : SystemBase
    {
        private double lastUpdateTime;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfig>();
            RequireSingletonForUpdate<Temperature>();
            
        }

        protected override void OnUpdate()
        {            
            var configEntity = GetSingletonEntity<FireSimConfig>();
            var configValues = GetComponent<FireSimConfigValues>(configEntity);

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

            VerticalSpreadJob verticalSpreadJob = new VerticalSpreadJob
            {
                Source = temperatureArray,
                Destination = intermediateArray,
                Columns = configValues.Columns,
                Rows = configValues.Rows,
                HeatRadius = configValues.HeatRadius,
                HeatTransferRate = configValues.HeatTransferRate,
                Flashpoint = configValues.Flashpoint,
            };

            Dependency = verticalSpreadJob.Schedule(temperatureBuffer.Length, 32, Dependency);

            HorizontalSpreadJob horizontalSpreadJob = new HorizontalSpreadJob
            {
                Source = intermediateArray,
                Destination = temperatureArray,
                Columns = configValues.Columns,
                Rows = configValues.Rows,
                HeatRadius = configValues.HeatRadius,
                HeatTransferRate = configValues.HeatTransferRate,
                Flashpoint = configValues.Flashpoint,
            };

            Dependency = horizontalSpreadJob.Schedule(temperatureBuffer.Length, 32, Dependency);

            intermediateArray.Dispose(Dependency);
        }
    }

    struct HorizontalSpreadJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Temperature> Source;

        public NativeArray<Temperature> Destination;

        public int Columns;
        public int Rows;
        public int HeatRadius;
        public float HeatTransferRate;
        public float Flashpoint;

        public void Execute(int index)
        {
            int column = index % Columns;
            int row = index / Columns;

            float temperature = Source[column + row * Columns].Intensity;

            for (int spread = -HeatRadius; spread <= HeatRadius; ++spread)
            {
                if (spread != 0 &&
                    spread + column >= 0 &&
                    spread + column < Columns)
                {
                    float spreadTemperature = Source[column + spread + row * Columns].Intensity;

                    if (spreadTemperature > Flashpoint)
                    {
                        temperature += spreadTemperature * HeatTransferRate;
                    }
                }
            }

            Destination[column + row * Columns] = new Temperature { Intensity = temperature };
        }
    }

    struct VerticalSpreadJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Temperature> Source;
        public NativeArray<Temperature> Destination;
        public int Columns;
        public int Rows;
        public int HeatRadius;
        public float HeatTransferRate;
        public float Flashpoint;

        public void Execute(int index)
        {
            int column = index % Columns;
            int row = index / Columns;

            float temperature = Source[column + row * Columns].Intensity;

            for (int spread = -HeatRadius; spread <= HeatRadius; ++spread)
            {
                if (spread != 0 &&
                    spread + row >= 0 &&
                    spread + row < Rows)
                {
                    float spreadTemperature = Source[column + (row + spread) * Columns].Intensity;

                    if (spreadTemperature > Flashpoint)
                    {
                        temperature += spreadTemperature * HeatTransferRate;
                    }
                }
            }

            Destination[column + row * Columns] = new Temperature { Intensity = temperature };

        }
    }
}
