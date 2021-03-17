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
        private EntityQuery query;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var metro = this.GetSingleton<GameObjectRefs>();
            
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();
            var ecbTags = CommandBufferSystem.CreateCommandBuffer();

            // the queuing tag is added as soon as we switch platforms and start moving, make sure we are done moving (WalkingTag) before processing the queue 
            Entities.WithStoreEntityQueryInField(ref query).WithoutBurst().WithAll<QueueingToEmbarkTag>().WithNone<WalkingTag>().ForEach((
                ref Translation translation,
                in SwitchingPlatformData platformData,
                in Entity commuter
                ) =>
            {
                int currentPlatform = platformData.platformTo;

                CommuterNavPoint queuePoint = metro.metro.allPlatforms[currentPlatform].queuePoints[0];
                
                ecb.AppendToBuffer(commuter, new PathData
                {
                    point = queuePoint.transform.position
                });
                
                ecb.AddComponent<WalkingTag>(commuter);
                ecb.AddComponent<SwitchingPlatformTag>(commuter);
                ecb.RemoveComponent<QueueingToEmbarkTag>(commuter);

            }).Run();
            
            //ecbTags.AddComponent<WalkingTag>(query);
            //ecbTags.AddComponent<SwitchingPlatformTag>(query);
            //ecbTags.RemoveComponent<QueueingToEmbarkTag>(query);
        }
    }
}