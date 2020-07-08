using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireRenderSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var bufferRef = EntityManager.GetBuffer<FireCell>(fireGridEntity);
        Entities
        .WithAll<FireRendererTag>()
        .ForEach((int entityInQueryIndex, ref NonUniformScale scale, ref Translation translation, ref FireColor fireColor) =>
        {
            // Grab the target cell this renderer should be matching
            FireCell currentCell = bufferRef[entityInQueryIndex];
            // Update its scale
            scale.Value.y = math.clamp(currentCell.FireTemperature, 0.001f, 1.0f);
            // Update its translation
            translation.Value.y = currentCell.FireTemperature * 0.5f;
            // Update its color
            fireColor.Value = math.lerp(new float4(0.0f, 1.0f, 0.0f, 1.0f), new float4(1.0f, 0.0f, 0.0f, 1.0f), currentCell.FireTemperature);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
