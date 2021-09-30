using System.Linq;
using Unity.Entities;
using Unity.Rendering;

public partial class TrainRefPropagationSystem : SystemBase
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

        ecb.RemoveComponentForEntityQuery<PropagateTrainRef>(RequirePropagation);
    
        var cdfe = GetComponentDataFromEntity<TrainReference>();

        Entities
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateTrainRef>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in TrainReference trainReference) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    if(HasComponent<TrainReference>(group[i].Value))
                        cdfe[group[i].Value] = trainReference;
                }
            }).ScheduleParallel();
    }
}