using Unity.Entities;
using Unity.Rendering;

public partial class StairColorPropagationSystem : SystemBase
{
    // Every Entities.ForEach relies on an implicit EntityQuery. This query depends on
    // the parameters of the lambda function used by the Entities.ForEach, and on the
    // additional "WithXXX" calls.
    // Sometimes, it is convenient to extract this implicit query from an Entity.ForEach.
    // For example, in order to use an EntityManager function or record a command to an
    // EntityCommandBuffer that requires a query to process a bunch of entities at once.
    // This is what this system does, and in order to extract the implicit query it has
    // to be stored in a field on the system itself.
    // See how it is used by WithStoreEntityQueryInField in OnUpdate.
    private EntityQuery RequirePropagation;
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        // Looking up another system in the world is an expensive operation.
        // In order to not do it every frame, we store the reference in a field.
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // An ECB system will play back later in the frame the buffers it creates.
        // This is useful to defer the structural changes that would cause sync points.
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        // We use this ECB to remove the tag component that requires color propagation.
        // If this line was missing, we would be redoing the propagation every frame.
        // Note that this isn't affecting the ForEach below, the change will only happen
        // when the ECB system runs, later in the frame.
        ecb.RemoveComponentForEntityQuery<PropagateColor>(RequirePropagation);
        //ecb.RemoveComponentForEntityQuery<URPMaterialPropertyBaseColor>(RequirePropagation);

        // A "ComponentDataFromEntity" allows random access to a component type from a job.
        // This is significantly slower than accessing the components from the current
        // entity via the lambda parameters.
        var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

        Entities
            // Random access to components for writing can be a race condition.
            // Here, we know for sure that prefabs don't share their entities.
            // So explicitly request to disable the safety system on the CDFE.
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithStoreEntityQueryInField(ref RequirePropagation)
            .WithAll<PropagateColor>()
            .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                , in URPMaterialPropertyBaseColor color) =>
            {
                for (int i = 1; i < group.Length; ++i)
                {
                    if(HasComponent<URPMaterialPropertyBaseColor>(group[i].Value))
                        cdfe[group[i].Value] = color;
                }
            }).ScheduleParallel();
    }
}