using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class FireSwapBufferSystem : SystemBase
{
    protected override void OnCreate()
    {
        GetEntityQuery(typeof(FireCell));
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var bufferCurrent = EntityManager.GetBuffer<FireCell>(fireGridEntity).AsNativeArray();

        Entities
        .WithReadOnly(bufferCurrent)
        .ForEach((DynamicBuffer<FireCellHistory> bufferHist) =>
        {
            for (int i = 0; i < bufferHist.Length; ++i)
            {
                FireCellHistory cell = bufferHist[i];
                cell.FireTemperaturePrev = bufferCurrent[i].FireTemperature;
                bufferHist[i] = cell;
            }
        }).Schedule();
    }
}
