using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class PlatformQueueManagementSystem : SystemBase
    {
        private EntityCommandBufferSystem CommandBufferSystem;
        
        //EntityQuery query;

        protected override void OnCreate()
        {
            CommandBufferSystem
                = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {

            var queryDesc = new EntityQueryDesc
            {
                None = new ComponentType[] {typeof(WalkingTag)},
                All = new ComponentType[] {typeof(LookingForQueueTag), typeof(SwitchingPlatformData)}
            };
            
            EntityQuery query = GetEntityQuery(queryDesc);


            NativeArray<Entity> commuters = query.ToEntityArray(Allocator.TempJob);
            NativeArray<SwitchingPlatformData> commuterPlatformData = query.ToComponentDataArray<SwitchingPlatformData>(Allocator.TempJob);
            
            
            var metro = this.GetSingleton<GameObjectRefs>();
            
            var deltaTime = Time.DeltaTime;
            var ecb = CommandBufferSystem.CreateCommandBuffer();

            // the queuing tag is added as soon as we switch platforms and start moving, make sure we are done moving (WalkingTag) before processing the queue 
            Entities.WithoutBurst().ForEach((
                in Entity queue,
                in PlatformQueue queueInfo
                ) =>
            {
                int platformIndex = queueInfo.platformIndex;


                for (int i = 0; i < commuters.Length; ++i)
                {
                    if (commuterPlatformData[i].platformTo == platformIndex)
                    {
                        
                        ecb.RemoveComponent<LookingForQueueTag>(commuters[i]);
                        ecb.AddComponent<QueueingForTrainTag>(commuters[i]);
                        
                        // add to list
                        ecb.AppendToBuffer<CommuterQueueData>(queue, new CommuterQueueData()
                        {
                            entity = commuters[i]
                        });

// --------------------- testing that it moves-----------------
                        int currentPlatform = platformIndex;

                        CommuterNavPoint queuePoint = metro.metro.allPlatforms[currentPlatform].queuePoints[0];
                
                        ecb.AppendToBuffer(commuters[i], new PathData
                        {
                            point = queuePoint.transform.position
                        });
                
                        ecb.AddComponent<WalkingTag>(commuters[i]);
// ---------------------------------------------
                        
                        //Queu
                        
                    }
                }
                

            }).Run();

            commuters.Dispose();
            commuterPlatformData.Dispose();
        }
    }
}