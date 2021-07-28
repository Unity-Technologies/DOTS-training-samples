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

            Color color_fireCell_neutral = Color.green;
            Color color_fireCell_cool = Color.red;
            Color color_fireCell_hot = Color.yellow;

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

                   float xPosition = (float)(column) * cellSize;
                   float yPosition = (float)(row) * cellSize;
                   float scaleX = cellSize;
                   float scaleY = configValues.MaxFlameHeight * temperature * cellSize;
                   float scaleZ = cellSize;

                   localToWorld.Value = float4x4.TRS(new float3(xPosition, 0, yPosition), Quaternion.identity, new Vector3(scaleX, scaleY, scaleZ));

                   Color cellColor = color_fireCell_neutral;
                   if (temperature >= configValues.Flashpoint)
                   {
                       cellColor = Color.Lerp(color_fireCell_cool, color_fireCell_hot, temperature);
                   }

                   color.Value = new float4(cellColor.r, cellColor.g, cellColor.b, 1.0f);
               }

           }).ScheduleParallel(Dependency);

            Dependency = updateFireCells;
        }
    }
}