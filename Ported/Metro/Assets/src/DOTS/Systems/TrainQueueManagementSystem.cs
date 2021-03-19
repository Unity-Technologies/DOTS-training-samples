using src.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace src.DOTS.Systems
{
    public class TrainQueueManagementSystem : SystemBase
    {

    
        private EndSimulationEntityCommandBufferSystem m_EndSimulationSystem;
        protected override void OnCreate()
        {
            m_EndSimulationSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        protected override void OnUpdate()
        {
            var metroBlob = this.GetSingleton<MetroBlobContainer>();

        var carriageFromEntity = GetComponentDataFromEntity<Carriage>(true);
        var ecb = m_EndSimulationSystem.CreateCommandBuffer().AsParallelWriter();
        var platformQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(PlatformQueue), ComponentType.ReadOnly<Translation>(), }
                
        };
        EntityQuery platformQuery = GetEntityQuery(platformQueryDesc);
        NativeArray<Entity> platformEntities = platformQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<PlatformQueue> allPlatformQueues = platformQuery.ToComponentDataArray<PlatformQueue>(Allocator.TempJob);
        NativeArray<Translation> allPlatformQueuePositions = platformQuery.ToComponentDataArray<Translation>(Allocator.TempJob); //Temp hack, written in 1985
        var firstPassengerFromEntity = GetComponentDataFromEntity<FirstQueuePassenger>(false);
        var movingFromEntity = GetComponentDataFromEntity<WalkingTag>(false);
        
        Entities
            .WithNativeDisableParallelForRestriction(firstPassengerFromEntity)
            .WithNativeDisableParallelForRestriction(movingFromEntity)
            .WithReadOnly(allPlatformQueuePositions)
            .WithReadOnly(platformEntities)
            .ForEach((Entity entity, int entityInQueryIndex, ref TrainWaiting trainWaiting, ref DynamicBuffer<CommuterQueueData> commuterQueue, in Translation translation, in Carriage carriage) => 
        {
           if(trainWaiting.platformEntity == Entity.Null)
           {
               
                //HACK!
                float distance = float.MaxValue;
                int destinationPlatform = 0;
                for (int i = 0; i < allPlatformQueues.Length; ++i)
                {
                    float lenSqr = math.lengthsq(translation.Value - allPlatformQueuePositions[i].Value);
                    if ( lenSqr< distance)
                    {
                        distance = lenSqr;
                        destinationPlatform = i;
                    }
                }
                
                //HACK
                trainWaiting.platformEntity = platformEntities[destinationPlatform];
                
                
                //Everyone, out!
                for (int i = 0; i < commuterQueue.Length; ++i)
                {
                    Entity ent = commuterQueue[i].entity;
                        
                    ecb.RemoveComponent<Parent>(entityInQueryIndex, ent);
                    ecb.RemoveComponent<LocalToParent>(entityInQueryIndex, ent);
                    ecb.AddComponent<SwitchingPlatformTag>(entityInQueryIndex, ent);
                    ecb.SetComponent(entityInQueryIndex, ent, new SwitchingPlatformData()
                    {
                        platformFrom = carriage.NextPlatformIndex,
                        platformTo = metroBlob.Blob.Value.Platforms[carriage.NextPlatformIndex].oppositePlatformIndex
                    });
                    
                    ecb.SetComponent(entityInQueryIndex, ent, new Translation() { Value = allPlatformQueuePositions[destinationPlatform].Value + new float3(i * 0.01f, 0.0f, 0.0f)});
                }
                commuterQueue.Clear();
           }
           else
           {
               //move player to position
               FirstQueuePassenger passengerToPickUp = firstPassengerFromEntity[trainWaiting.platformEntity];
               //ecb.AddComponent<SwitchingPlatformTag>(entityInQueryIndex, passengerToPickUp.passenger);
               if (trainWaiting.PassengersToTake > 0 && passengerToPickUp.passenger != Entity.Null && !movingFromEntity.HasComponent(passengerToPickUp.passenger))
               {
                   commuterQueue.Add(new CommuterQueueData{ entity = passengerToPickUp.passenger});
                   ecb.AddComponent<Parent>(entityInQueryIndex, passengerToPickUp.passenger, new Parent() {Value = entity});
                   ecb.AddComponent<LocalToParent>(entityInQueryIndex, passengerToPickUp.passenger);
                   ecb.SetComponent(entityInQueryIndex, passengerToPickUp.passenger, new Translation() {Value = new float3(0.0f, 0.0f, 0.0f)});
                   ecb.SetComponent(entityInQueryIndex, trainWaiting.platformEntity, new FirstQueuePassenger() {passenger = Entity.Null});
                   trainWaiting.PassengersToTake--;
               }
               
        
               //
           }

        })
            .WithDisposeOnCompletion(platformEntities)
            .WithDisposeOnCompletion(allPlatformQueues)
            .WithDisposeOnCompletion(allPlatformQueuePositions)
            .ScheduleParallel();
        
        m_EndSimulationSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}