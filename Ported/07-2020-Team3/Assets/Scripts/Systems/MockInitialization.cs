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
    private EntityQuery _carQuery;
    private EntityQuery _commuterQuery;

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

        _carQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TrainCar>()
            }
        });

        _commuterQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Commuter>()
            }
        });
    }

    
    protected override void OnUpdate()
    {


        if (!HasSingleton<SeatsPerCar>())
            return;

        var trackEntities = _trackQuery.ToEntityArray(Allocator.TempJob);
        var trackPointsAccessor = GetBufferFromEntity<TrackPoint>(false);
        var carEntities = _carQuery.ToEntityArray(Allocator.TempJob);
        var commuterEntities = _commuterQuery.ToEntityArray(Allocator.TempJob);
        var seatAccessor = GetBufferFromEntity<Seat>();
        
        EntityCommandBuffer.Concurrent ecb = _ecb.CreateCommandBuffer().ToConcurrent();

        var seatsPerCar = GetSingleton<SeatsPerCar>();

        Entities
            .WithDeallocateOnJobCompletion(trackEntities)
            .WithDeallocateOnJobCompletion(carEntities)
            .WithDeallocateOnJobCompletion(commuterEntities)
            .ForEach((int entityInQueryIndex, in Entity entity, in InitializationEvent initializationEvent) =>
        {
            int commuterIndex = 0;
            
            

            for (int i = 0; i < trackEntities.Length; i++)
            {
                var trackEntity = trackEntities[i];
                var trackPoints = trackPointsAccessor[trackEntity];
                
                /*
                Debug.Log("Set Track");
                trackPoints.Clear();
                for (int j = 0; j < 16; j++)
                    trackPoints.Add(new TrackPoint { position = 100.0f * new float3(math.sin(2.0f * j * math.PI/16), 0, math.cos(2.0f * j * math.PI/16)) });
                */

                DynamicBuffer<TrackPlatforms> trackPlatforms = ecb.AddBuffer<TrackPlatforms>(entityInQueryIndex, trackEntity);
                CreatePlatform(entityInQueryIndex, ref ecb, ref trackPlatforms, 1.5f);
                CreatePlatform(entityInQueryIndex, ref ecb, ref trackPlatforms, 5.5f);
                CreatePlatform(entityInQueryIndex, ref ecb, ref trackPlatforms, 8.5f);

            }

            
            for(int i = 0; i < carEntities.Length; i++)
            {
                Entity carEntity = carEntities[i];
                var seats = ecb.AddBuffer<Seat>(entityInQueryIndex, carEntity);

                for (int j = 0; j < seatsPerCar.cols * seatsPerCar.rows; j++)
                {
                    seats.Add(new Seat
                    {
                        occupiedBy = commuterIndex < commuterEntities.Length ? commuterEntities[commuterIndex] : Entity.Null
                    });
                }
            }


            

            ecb.DestroyEntity(entityInQueryIndex, entity);
        }).Schedule();

        _ecb.AddJobHandleForProducer(Dependency);
        
        
    }
    private static void CreatePlatform(int entityInQueryIndex, ref EntityCommandBuffer.Concurrent ecb, ref DynamicBuffer<TrackPlatforms> trackPlatforms, float position)
    {
        trackPlatforms.Add(new TrackPlatforms
        {
            platform = Entity.Null,
            position = position
        });

        Entity platform = ecb.CreateEntity(entityInQueryIndex);
        Entity platformWaypoint = ecb.CreateEntity(entityInQueryIndex);
        
    }

}
