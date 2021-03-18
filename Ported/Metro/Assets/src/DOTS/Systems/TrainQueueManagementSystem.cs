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

        Entities
            .WithNativeDisableParallelForRestriction(firstPassengerFromEntity)
            .WithReadOnly(allPlatformQueuePositions)
            .WithReadOnly(platformEntities)
            .ForEach((Entity entity, int entityInQueryIndex, ref TrainWaiting trainWaiting, in Translation translation) => 
        {
           if(trainWaiting.platformEntity == Entity.Null)
           {
               
                //HACK!
                float distance = float.MaxValue;
                int destinationPlatform = 0;
                for (int i = 0; i < allPlatformQueues.Length; ++i)
                {
                    if (math.lengthsq(translation.Value - allPlatformQueuePositions[i].Value) < distance)
                    {
                        destinationPlatform = i;
                    }
                }
                
                //HACK
                trainWaiting.platformEntity = platformEntities[destinationPlatform];


           }
           else
           {
               //move player to position
               FirstQueuePassenger passengerToPickUp = firstPassengerFromEntity[trainWaiting.platformEntity];
               //ecb.AddComponent<SwitchingPlatformTag>(entityInQueryIndex, passengerToPickUp.passenger);
               passengerToPickUp.passenger = Entity.Null;
        
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