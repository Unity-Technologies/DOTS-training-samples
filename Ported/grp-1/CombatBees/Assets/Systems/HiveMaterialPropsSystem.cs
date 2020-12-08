using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class HiveMaterialPropsSystem : SystemBase
{
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
       // // This is useful to defer the structural changes that would cause sync points.
       // var ecb = CommandBufferSystem.CreateCommandBuffer();
       //
       // // We use this ECB to remove the tag component that requires color propagation.
       // // If this line was missing, we would be redoing the propagation every frame.
       // // Note that this isn't affecting the ForEach below, the change will only happen
       // // when the ECB system runs, later in the frame.
       // ecb.RemoveComponent<HiveMaterialSetProps>(RequirePropagation);
       //
       // // A "ComponentDataFromEntity" allows random access to a component type from a job.
       // // This much slower than accessing the components from the current entity via the
       // // lambda parameters.
       // var cdfe0 = GetComponentDataFromEntity<URPMaterialPropertyHideColor>();
       // var cdfe1 = GetComponentDataFromEntity<URPMaterialPropertyHideTeam0Color>();
       // var cdfe2 = GetComponentDataFromEntity<URPMaterialPropertyHideTeam1Color>();
       //
       // Entities
       //     // Random access to components for writing can be a race condition.
       //     // Here, we know for sure that prefabs don't share their entities.
       //     // So explicitly request to disable the safety system on the CDFE.
       //     .WithNativeDisableContainerSafetyRestriction(cdfe0)
       //     .WithStoreEntityQueryInField(ref RequirePropagation)
       //     .WithAll<HiveMaterialSetProps>()
       //     .ForEach((in DynamicBuffer<LinkedEntityGroup> group
       //         , in URPMaterialPropertyHideColor color) =>
       //     {
       //         for (int i = 1; i < group.Length; ++i)
       //         {
       //             cdfe0[group[i].Value] = color;
       //         }
       //     }).ScheduleParallel();
       //
       // Entities
       //     // Random access to components for writing can be a race condition.
       //     // Here, we know for sure that prefabs don't share their entities.
       //     // So explicitly request to disable the safety system on the CDFE.
       //     .WithNativeDisableContainerSafetyRestriction(cdfe1)
       //     .WithStoreEntityQueryInField(ref RequirePropagation)
       //     .WithAll<HiveMaterialSetProps>()
       //     .ForEach((in DynamicBuffer<LinkedEntityGroup> group
       //         , in URPMaterialPropertyHideTeam0Color color) =>
       //     {
       //         for (int i = 1; i < group.Length; ++i)
       //         {
       //             cdfe1[group[i].Value] = color;
       //         }
       //     }).ScheduleParallel();
    }
}