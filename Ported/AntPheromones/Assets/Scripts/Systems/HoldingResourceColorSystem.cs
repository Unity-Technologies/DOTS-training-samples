using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class HoldingResourceColorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var settings = GetComponent<GeneralSettings>(GetSingletonEntity<GeneralSettings>());
        
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var parallelWriter = ecb.AsParallelWriter();

        // A "ComponentDataFromEntity" allows random access to a component type from a job.
        // This is significantly slower than accessing the components from the current
        // entity via the lambda parameters.
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithAll<RemoveHoldingResourceColor>()
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color, in Brightness brightness, in DynamicBuffer<LinkedEntityGroup> group) =>
            {
                color.Value = new float4(settings.SearchColor.Value.x*brightness.Value, settings.SearchColor.Value.y*brightness.Value, settings.SearchColor.Value.z*brightness.Value, 1f);
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
                parallelWriter.RemoveComponent<RemoveHoldingResourceColor>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        
        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithAll<AddHoldingResourceColor>()
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color, in Brightness brightness, in DynamicBuffer<LinkedEntityGroup> group) =>
            {
                color.Value = new float4(settings.CarryColor.Value.x*brightness.Value, settings.CarryColor.Value.y*brightness.Value, settings.CarryColor.Value.z*brightness.Value, 1f);
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
                parallelWriter.RemoveComponent<AddHoldingResourceColor>(entityInQueryIndex, entity);
            }).ScheduleParallel();
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
