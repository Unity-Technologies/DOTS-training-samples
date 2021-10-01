using System.Linq;
using dots_src.Components;
using Unity.Entities;
using Unity.Rendering;

public partial class PlatformRefPropagationSystem : SystemBase
{
    private EntityQuery RequirePropagation;
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {

        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
   
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        //ecb.RemoveComponentForEntityQuery<PropagatePlatformRef>(RequirePropagation);
    
        var cdfe = GetComponentDataFromEntity<PlatformRef>();

        Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagatePlatformRef>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group, in PlatformRef platformRef) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    if(HasComponent<PlatformRef>(group[i].Value))
                        cdfe[group[i].Value] = platformRef;
                }
            }).ScheduleParallel();
    }
}