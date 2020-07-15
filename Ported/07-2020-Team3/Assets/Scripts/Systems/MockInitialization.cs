using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

public class MockInitialization : SystemBase
{

    private EntityArchetype _trainArchetype;
    private EntityCommandBufferSystem _ecb;
    private EntityQuery _trackQuery;

    protected override void OnCreate()
    {
        _ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        _trackQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TrackPoint>()
            }
        });


        _trainArchetype = EntityManager.CreateArchetype(typeof(TrainPosition));
    }

    
    protected override void OnUpdate()
    {
        /*
        var trackEntities = _trackQuery.ToEntityArrayAsync(Allocator.TempJob, out var trackEntitiesHandle);
        
        BufferFromEntity<TrackPoint> lookup = GetBufferFromEntity<TrackPoint>();

        Dependency = JobHandle.CombineDependencies(Dependency, trackEntitiesHandle);

        EntityCommandBuffer.Concurrent ecb = _ecb.CreateCommandBuffer().ToConcurrent();

        EntityArchetype trainArchetype = _trainArchetype;

        Entities
            .WithDeallocateOnJobCompletion(trackEntities)
            .ForEach((int entityInQueryIndex, in Entity entity, in InitializationEvent initializationEvent) =>
        {

            for(int i = 0; i < trackEntities.Length; i++)
            {
                Entity trackEntity = trackEntities[i];

                Entity trainEntity = ecb.CreateEntity(entityInQueryIndex, trainArchetype);
                TrainPosition trainPosition = new TrainPosition
                {
                    track = trackEntity,
                    position = 0.0f,
                    speed = 0.1f,
                };
                ecb.SetComponent(entityInQueryIndex, trainEntity, trainPosition);


                for(int j = 0; j < 5; j++)
                {
                    Entity carEntity = ecb.CreateEntity(entityInQueryIndex);
                    TrainCar carCmp = new TrainCar
                    {
                        train = trainEntity,
                        indexInTrain = j,
                    };
                    ecb.AddComponent<TrainCar>(entityInQueryIndex, carEntity);
                }

            }

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule();

        _ecb.AddJobHandleForProducer(Dependency);
        
        //*/
    }

}
