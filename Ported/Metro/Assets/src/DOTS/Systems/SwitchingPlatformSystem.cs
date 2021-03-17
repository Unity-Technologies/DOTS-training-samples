using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class SwitchingPlatformSystem : SystemBase
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
            var metro = this.GetSingleton<GameObjectRefs>();
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithoutBurst().ForEach((in SwitchingPlatformTag switchingPlatformTag, in Entity commuter) =>
            {
                
                var from = metro.metro.allPlatforms[switchingPlatformTag.platformFrom];
                var to = metro.metro.allPlatforms[switchingPlatformTag.platformTo];
                
                
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway_BACK_CROSS.nav_START.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway_BACK_CROSS.nav_END.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway_FRONT_CROSS.nav_END.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway_FRONT_CROSS.nav_START.transform.position
                });
                ecb.AddComponent(commuter, new CurrentPathTarget
                {
                    currentIndex = 0,
                });
                
                
                ecb.RemoveComponent<SwitchingPlatformTag>(commuter);
                
                //int targetPlatform = platformTag.platformTo;
                //ecb.AddComponent<QueueingToEmbarkTag>(commuter, new QueueingToEmbarkTag()
                //{
                //    platform = targetPlatform,
                //});
                
            }).Run();
        }
    }
}