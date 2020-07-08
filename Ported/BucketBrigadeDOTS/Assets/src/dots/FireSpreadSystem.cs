using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireSpreadSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var bufferPrev = EntityManager.GetBuffer<FireCellHistory>(fireGridEntity);

        Entities
        .WithReadOnly(bufferPrev)
        .ForEach((DynamicBuffer<FireCell> buffer) =>
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                FireCell cell = buffer[i];
                cell.FireTemperature = bufferPrev[i].FireTemperaturePrev + 0.01f;
                buffer[i] = cell;
            }
        }).Schedule();
    }
}
