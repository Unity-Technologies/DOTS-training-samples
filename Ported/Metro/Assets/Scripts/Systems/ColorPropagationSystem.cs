using System.Text.RegularExpressions;
using Unity.Entities;
using Unity.Rendering;

public partial class ColorPropagationSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer();
        
        ecb.RemoveComponentForEntityQuery<PropagateColor>(RequirePropagation);

        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group, in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel();
    }
}