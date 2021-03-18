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
            var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

            Entities.WithStoreEntityQueryInField(ref query)
                .WithAll<SwitchingPlatformTag>()
                .WithNone<WalkingTag>()
                .ForEach((
                    Entity commuter,
                    int entityInQueryIndex,
                    in SwitchingPlatformData switchingPlatformData
                    ) =>
            {
                var from = blob.Blob.Value.Platforms[switchingPlatformData.platformFrom];
                var to = blob.Blob.Value.Platforms[switchingPlatformData.platformTo];

                // inserting in reverse order
                ecb.AppendToBuffer(entityInQueryIndex, commuter, new PathData
                {
                    point = to.walkway.frontStart
                });
                ecb.AppendToBuffer(entityInQueryIndex, commuter, new PathData
                {
                    point = to.walkway.frontEnd
                });
                ecb.AppendToBuffer(entityInQueryIndex, commuter, new PathData
                {
                    point = from.walkway.backEnd
                });
                ecb.AppendToBuffer(entityInQueryIndex, commuter, new PathData
                {
                    point = from.walkway.backStart
                });

                ecb.AddComponent<WalkingTag>(entityInQueryIndex, commuter);
                ecb.AddComponent<LookingForQueueTag>(entityInQueryIndex, commuter);
                ecb.RemoveComponent<SwitchingPlatformTag>(entityInQueryIndex, commuter);
            }).ScheduleParallel();
            
            CommandBufferSystem.AddJobHandleForProducer(Dependency);
            
            // sync version
            //EntityManager.RemoveComponent<SwitchingPlatformTag>(query);
            
            //ecbTags.RemoveComponent<SwitchingPlatformTag>(query);
            //ecbTags.AddComponent<QueueingToEmbarkTag>(query);
            //ecbTags.AddComponent<WalkingTag>(query);
        }
    }
}