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
        private EntityQuery query;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var metro = this.GetSingleton<GameObjectRefs>();
            var ecb = CommandBufferSystem.CreateCommandBuffer();
            var ecbTags = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithStoreEntityQueryInField(ref query).WithoutBurst().WithAll<SwitchingPlatformTag>().WithNone<WalkingTag>().ForEach((in SwitchingPlatformData switchingPlatformData, in Entity commuter) =>
            {
                
                var from = metro.metro.allPlatforms[switchingPlatformData.platformFrom];
                var to = metro.metro.allPlatforms[switchingPlatformData.platformTo];
                
                // inserting in reverse order
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway_FRONT_CROSS.nav_START.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway_FRONT_CROSS.nav_END.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway_BACK_CROSS.nav_END.transform.position
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway_BACK_CROSS.nav_START.transform.position
                });
                
                ecb.AddComponent<WalkingTag>(commuter);
                ecb.AddComponent<LookingForQueueTag>(commuter);
                ecb.RemoveComponent<SwitchingPlatformTag>(commuter);
                
            }).Run();

            // sync version
            //EntityManager.RemoveComponent<SwitchingPlatformTag>(query);
            
            //ecbTags.RemoveComponent<SwitchingPlatformTag>(query);
            //ecbTags.AddComponent<QueueingToEmbarkTag>(query);
            //ecbTags.AddComponent<WalkingTag>(query);
        }
    }
}