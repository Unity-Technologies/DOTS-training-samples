using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class SetupPlayerColorsSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    // Should probably not run every frame
    protected override void OnUpdate()
    {
        Dependency = Entities.ForEach((Entity e, ref URPMaterialPropertyBaseColor urpColor, in PlayerIndex playerIndex) =>
        {
            if(playerIndex.Value == 0)
                urpColor.Value = new float4(1.0f, 0.0f, 0.0f, 1.0f);
            else if (playerIndex.Value == 1)
                urpColor.Value = new float4(0.0f, 1.0f, 0.0f, 1.0f);
            else if (playerIndex.Value == 2)
                urpColor.Value = new float4(0.0f, 0.0f, 1.0f, 1.0f);
            else
                urpColor.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
        }).ScheduleParallel(Dependency);
        
        
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        
        Dependency = Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel(Dependency);
    }
}
