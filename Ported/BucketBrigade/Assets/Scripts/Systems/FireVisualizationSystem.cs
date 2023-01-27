using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSimSystem))]
[BurstCompile]
partial struct FireVisualizationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var lowTempColor = config.lowTemperatureColour;
        var highTempColor = config.highTemperatureColour;

        foreach (var flameCellAspect in SystemAPI.Query<FireCellAspect>())
        {
            var flameCell = flameCellAspect.Self;
            var displayHeight = flameCellAspect.DisplayHeight.ValueRO.height;
            var cellNum = flameCellAspect.CellInfo.ValueRO.indexX + flameCellAspect.CellInfo.ValueRO.indexY;
            var timeAdjusted = ((float) SystemAPI.Time.ElapsedTime);
            var cellOffset = new float2(cellNum, timeAdjusted);
            var randomOffset = noise.cnoise(cellOffset);
            var lerpedColor = Color.Lerp(lowTempColor, highTempColor, displayHeight);

            if (displayHeight > 0.05f)
            {
                var scale = new float3(1, (displayHeight * 10 + 1) + randomOffset, 1);
                var postTransformScale = new PostTransformScale() {Value = float3x3.Scale(scale)};
                var cellColor = new URPMaterialPropertyBaseColor() {Value = (Vector4) lerpedColor};
                SystemAPI.SetComponent(flameCell, cellColor);
                SystemAPI.SetComponent(flameCell, postTransformScale);
            }
        }
    }
}
