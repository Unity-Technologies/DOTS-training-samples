using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using UnityEditorInternal;
using Unity.Mathematics;

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
                ComponentType.ReadWrite<TrackPoint>()
            }
        });
    }

    
    protected override void OnUpdate()
    {
        
        var trackEntities = _trackQuery.ToEntityArray(Allocator.TempJob);
        var trackPointsAccessor = GetBufferFromEntity<TrackPoint>(false);
        
        EntityCommandBuffer.Concurrent ecb = _ecb.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithDeallocateOnJobCompletion(trackEntities)
            .ForEach((int entityInQueryIndex, in Entity entity, in InitializationEvent initializationEvent) =>
        {

            for(int i = 0; i < trackEntities.Length; i++)
            {
                var trackEntity = trackEntities[i];
                var trackPoints = trackPointsAccessor[trackEntity];
                
                Debug.Log("Set Track");
                trackPoints.Clear();
                for (int j = 0; j < 16; j++)
                    trackPoints.Add(new TrackPoint { position = 100.0f * new float3(math.sin(2.0f * j * math.PI/16), 0, math.cos(2.0f * j * math.PI/16)) });

                 
                /*

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
                */

            }

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule();

        _ecb.AddJobHandleForProducer(Dependency);
        
        
    }

}
