using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class EmbarkingQueueSystem : SystemBase
    {
        private EntityCommandBufferSystem CommandBufferSystem;
        const float m_D = 1f;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().WithAll<SwitchingPlatformTag>().ForEach((
                ref Translation translation
                ) =>
            {
                
            }).Run();
        }
    }
}