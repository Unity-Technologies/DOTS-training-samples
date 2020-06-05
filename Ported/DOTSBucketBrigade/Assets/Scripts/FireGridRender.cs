using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireGridSimulate))]
public class FireGridRender : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    private const float kGroundHeight = 0.001f;
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<BucketBrigadeConfig>();
        RequireSingletonForUpdate<FireGrid>();

        m_EndSimulationEcbSystem = World
           .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();
        var gridEntity = GetSingletonEntity<FireGrid>();

        var array = EntityManager.GetBuffer<FireGridCell>(gridEntity).AsNativeArray();
        float flashpoint = config.Flashpoint;

        float time = (float)Time.ElapsedTime;

        float3 neutralinear = new float3(config.ColorNeutral.r, config.ColorNeutral.g, config.ColorNeutral.b);
        float4 neutralColor = new float4 ((neutralinear * (neutralinear * (neutralinear * 0.305306011f + 0.682171111f) + 0.012522878f)), 1);
        
        float3 coolLinear = new float3(config.ColorCool.r, config.ColorCool.g, config.ColorCool.b);
        float4 coolColor = new float4 ((coolLinear * (coolLinear * (coolLinear * 0.305306011f + 0.682171111f) + 0.012522878f)), 1);
        
        float3 hotLinear = new float3(config.ColorHot.r, config.ColorHot.g, config.ColorHot.b);
        float4 hotColor = new float4 ((hotLinear * (hotLinear * (hotLinear * 0.305306011f + 0.682171111f) + 0.012522878f)), 1);

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

        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
