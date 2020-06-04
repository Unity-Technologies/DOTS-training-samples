using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireGridSimulate))]
public class FireGridRender : SystemBase
{

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

        //pointerOfDoom = (FireGridCell*)buffer.GetUnsafeReadOnlyPtr();

        float flashpoint = config.Flashpoint;

        //var localDoom = pointerOfDoom;

        Entities.WithNativeDisableContainerSafetyRestriction(array).ForEach((ref NonUniformScale Scale, in GridCellIndex Index) =>
        {
            FireGridCell cell = array[Index.Index];

            if (cell.Temperature > flashpoint)
            {
                Scale.Value.y = math.max(cell.Temperature, 0.001f);
            }
            else
            {
                Scale.Value.y = 0.001f;
            }
        }).ScheduleParallel();
    }
}
