using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSimSystem))]
[BurstCompile]
partial struct VisualizationSystem : ISystem
{
    EntityQuery m_water;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        m_water = state.GetEntityQuery(ComponentType.ReadWrite<WaterAmount>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var defaultColor = config.defaultTemperatureColour;
        var lowTempColor = config.lowTemperatureColour;
        var highTempColor = config.highTemperatureColour;

        foreach (var flameCellAspect in SystemAPI.Query<FireCellAspect>().WithAll<OnFireTag>())
        {
            var flameCell = flameCellAspect.Self;
            var displayHeight = flameCellAspect.DisplayHeight.ValueRO.height;
            var cellNum = flameCellAspect.CellInfo.ValueRO.indexX + flameCellAspect.CellInfo.ValueRO.indexY;
            var timeAdjusted = ((float) SystemAPI.Time.ElapsedTime);
            var cellOffset = new float2(cellNum, timeAdjusted);
            var randomOffset = noise.cnoise(cellOffset);
            var lerpedColor = Color.Lerp(lowTempColor, highTempColor, displayHeight);
            
            if (displayHeight > 0.01f)
            {
                var scale = new float3(1, (displayHeight * 10 + 1) + randomOffset, 1);
                var postTransformScale = new PostTransformScale() {Value = float3x3.Scale(scale)};
                var cellColor = new URPMaterialPropertyBaseColor() {Value = (Vector4) lerpedColor};
                SystemAPI.SetComponent(flameCell, postTransformScale);
                SystemAPI.SetComponent(flameCell, cellColor);
            }
            else
            {
                var cellScale = SystemAPI.GetComponent<PostTransformScale>(flameCell).Value.c1.y;
                if (cellScale is > 1 or < 1)
                {
                    var postTransformScale = new PostTransformScale() {Value = float3x3.Scale(1)};
                    var cellColor = new URPMaterialPropertyBaseColor() {Value = (Vector4) defaultColor};
                    SystemAPI.SetComponent(flameCell, postTransformScale);
                    SystemAPI.SetComponent(flameCell, cellColor);
                }
            }
        }

        var emptyBucketColor = config.emptyBucketColour;
        var fullBucketColor = config.fullBucketColour;


        foreach (var waterEntity in m_water.ToEntityArray(Allocator.Temp))
        {
            var waterAmount = SystemAPI.GetComponent<WaterAmount>(waterEntity).currentContain;
            var lerpBucketColor = Color.Lerp(emptyBucketColor, fullBucketColor, waterAmount);
            var bucketColor = new URPMaterialPropertyBaseColor() {Value = (Vector4)lerpBucketColor};
            SystemAPI.SetComponent(waterEntity,bucketColor);

            var transformAspect = SystemAPI.GetAspectRW<TransformAspect>(waterEntity);
            transformAspect.LocalScale = waterAmount * 0.02f + 1;
        }
    }
}
