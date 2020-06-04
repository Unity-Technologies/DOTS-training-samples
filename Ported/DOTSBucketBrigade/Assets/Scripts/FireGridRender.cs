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

        Entities.WithNativeDisableContainerSafetyRestriction(array).ForEach((ref NonUniformScale Scale, in GridCellIndex Index) =>
        {
            FireGridCell cell = array[Index.Index];
            float height = kGroundHeight;

            if (cell.Temperature > flashpoint)
            {
                height = math.max(cell.Temperature, kGroundHeight);
                height += (config.FlickerRange * 0.5f) + UnityEngine.Mathf.PerlinNoise((time - Index.Index) * config.FlickerRate - cell.Temperature, cell.Temperature) * config.FlickerRange;
            }

            Scale.Value.y = height;

        }).ScheduleParallel();
    }
}
