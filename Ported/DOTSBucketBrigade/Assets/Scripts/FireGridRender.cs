using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireGridSimulate))]
public class FireGridRender : SystemBase
{
    private const float kGroundHeight = 0.001f;
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<BucketBrigadeConfig>();
        RequireSingletonForUpdate<FireGrid>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();
        var gridEntity = GetSingletonEntity<FireGrid>();

        var array = EntityManager.GetBuffer<FireGridCell>(gridEntity).AsNativeArray();
        float flashpoint = config.Flashpoint;

        float time = (float)Time.ElapsedTime;
        float4 neutralColor = new float4(config.ColorNeutral.r, config.ColorNeutral.g, config.ColorNeutral.b, 1);
        float4 coolColor = new float4(config.ColorCool.r, config.ColorCool.g, config.ColorCool.b, 1);
        float4 hotColor = new float4(config.ColorHot.r, config.ColorHot.g, config.ColorHot.b, 1);

        Entities
            .WithNativeDisableContainerSafetyRestriction(array)
            .ForEach((ref NonUniformScale Scale, ref CubeColor cubeColor, in GridCellIndex Index) =>
        {
            FireGridCell cell = array[Index.Index];
            float height = kGroundHeight;
            float4 color = neutralColor;

            if (cell.Temperature > flashpoint)
            {
                height = math.max(cell.Temperature, kGroundHeight);
                height += (config.FlickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - Index.Index) * config.FlickerRate - cell.Temperature, cell.Temperature) * config.FlickerRange;
                color = math.lerp(coolColor, hotColor, cell.Temperature);
            }

            Scale.Value.y = height;

            cubeColor.Color = color;

        }).ScheduleParallel();
    }
}
