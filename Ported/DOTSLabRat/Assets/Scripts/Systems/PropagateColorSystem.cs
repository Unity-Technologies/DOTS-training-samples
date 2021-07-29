using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace DOTSRATS
{
    public class PropagateColorSystem : SystemBase
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
            ecb.RemoveComponentForEntityQuery<PropagateColor>(RequirePropagation);
            var cdfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();

            Entities
                .WithNativeDisableContainerSafetyRestriction(cdfe)
                .WithStoreEntityQueryInField(ref RequirePropagation)
                .WithAll<PropagateColor>()
                .WithoutBurst()
                .ForEach((in DynamicBuffer<LinkedEntityGroup> group
                    , in PropagateColor propagateColor) =>
                {
                    for (int i = 1; i < group.Length; ++i)
                    {
                        if (EntityManager.HasComponent<URPMaterialPropertyBaseColor>(group[i].Value))
                        {
                            cdfe[group[i].Value] = new URPMaterialPropertyBaseColor { Value = propagateColor.color };
                        }
                    }
                }).Run();
        }
    }
}