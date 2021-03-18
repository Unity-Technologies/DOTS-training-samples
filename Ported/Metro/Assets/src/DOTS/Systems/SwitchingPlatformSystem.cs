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
            var blob = this.GetSingleton<MetroBlobContainer>();
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            Entities.WithStoreEntityQueryInField(ref query).WithoutBurst()
                .WithAll<SwitchingPlatformTag>()
                .WithNone<WalkingTag>()
                .ForEach((
                    in SwitchingPlatformData switchingPlatformData,
                    in Entity commuter) =>
            {
                var from = blob.Blob.Value.Platforms[switchingPlatformData.platformFrom];
                var to = blob.Blob.Value.Platforms[switchingPlatformData.platformTo];

                // inserting in reverse order
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway.frontStart
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = to.walkway.frontEnd
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway.backEnd
                });
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = from.walkway.backStart
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