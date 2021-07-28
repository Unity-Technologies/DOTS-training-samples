using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;

namespace src.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class FireRenderSystem : SystemBase
    {
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

            var temperatureEntity = GetSingletonEntity<Temperature>();
            DynamicBuffer<Temperature> temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

            NativeArray<Temperature>.ReadOnly temperatureArray = temperatureBuffer.AsNativeArray().AsReadOnly();

            var cellSize = configValues.CellSize;

            Color color_fireCell_neutral = new Color(0.49f, 0.79f, 0.45f);
            Color color_fireCell_cool = new Color(1.0f, 0.98f, 0.51f);
            Color color_fireCell_hot = Color.red;

            double time = Time.ElapsedTime;

            var updateFireCells = Entities
                .WithName("UpdateFireCellPresentation")                
                .WithBurst()
                .WithAll<FireCellTag>()
                .ForEach((int entityInQueryIndex, ref LocalToWorld localToWorld, ref URPMaterialPropertyBaseColor color) =>
           {
               int column = entityInQueryIndex % configValues.Columns;
               int row = entityInQueryIndex / configValues.Columns;

               if (column < configValues.Columns && row < configValues.Rows)
               {
                   float temperature = temperatureArray[column + row * configValues.Columns].Intensity; //configValues.GetTemperatureForCell(temperatureBuffer, row, column);

                   float temperatureWithFlicker = temperature;
                   Color cellColor = color_fireCell_neutral;

                   if (temperature >= configValues.Flashpoint)
                   {
                       const float flickerAmount = 0.3f;
                       const float flickerRate = 0.1f;
                       float flickerNoise = Mathf.PerlinNoise(((float)entityInQueryIndex - (float)time) * flickerRate, temperature) - 0.5f;

                       temperatureWithFlicker = temperature - flickerAmount * flickerNoise; 

                       cellColor = Color.Lerp(color_fireCell_cool, color_fireCell_hot, temperatureWithFlicker);
                   }

                   var worldPosition = configValues.GetCellWorldPosition3D(row, column);
                   float xPosition = worldPosition.x + cellSize * 0.5f;
                   float zPosition = worldPosition.z + cellSize * 0.5f;
                   float yPosition = configValues.MaxFlameHeight * temperatureWithFlicker;
                   float scaleX = cellSize;
                   float scaleY = configValues.MaxFlameHeight; 
                   float scaleZ = cellSize;

                   localToWorld.Value = float4x4.TRS(new float3(xPosition, yPosition, zPosition), Quaternion.identity, new Vector3(scaleX, scaleY, scaleZ));

                   color.Value = new float4(cellColor.r, cellColor.g, cellColor.b, 1.0f);
               }

           }).ScheduleParallel(Dependency);

            Dependency = updateFireCells;
        }
    }
}