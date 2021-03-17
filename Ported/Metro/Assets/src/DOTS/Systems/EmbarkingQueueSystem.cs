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
            // TODO: ot needed anymore, remove it
            return;
            
            var metro = this.GetSingleton<GameObjectRefs>();
            
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();
            var ecbTags = CommandBufferSystem.CreateCommandBuffer();

            // the queuing tag is added as soon as we switch platforms and start moving, make sure we are done moving (WalkingTag) before processing the queue 
            //Entities.WithStoreEntityQueryInField(ref query).WithoutBurst().WithAll<LookingForQueueTag>().WithNone<WalkingTag>().ForEach((
            //    ref Translation translation,
            //    ref SwitchingPlatformData platformData,
            //    in Entity commuter
            //    ) =>
            //{
            //    int currentPlatform = platformData.platformTo;
//
            //    CommuterNavPoint queuePoint = metro.metro.allPlatforms[currentPlatform].queuePoints[0];
            //    
            //    ecb.AppendToBuffer(commuter, new PathData
            //    {
            //        point = queuePoint.transform.position
            //    });
            //    
            //    ecb.AddComponent<WalkingTag>(commuter);
            //    ecb.AddComponent<SwitchingPlatformTag>(commuter);
            //    ecb.RemoveComponent<LookingForQueueTag>(commuter);
            //    
            //    int targetPlatform = platformData.platformFrom;
//
            //    // HACK: for now switch the platforms like this so it just goes back to the platform it came from
            //    platformData.platformFrom = currentPlatform;
            //    platformData.platformTo = targetPlatform;
//
            //}).Run();
            
            //ecbTags.AddComponent<WalkingTag>(query);
            //ecbTags.AddComponent<SwitchingPlatformTag>(query);
            //ecbTags.RemoveComponent<QueueingToEmbarkTag>(query);
        }
    }
}