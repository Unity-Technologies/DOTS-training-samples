using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CommuterBoardTrainSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery m_TrainCarsQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        m_TrainCarsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TrainCar>(),
                ComponentType.ReadWrite<Seat>(),
            }
        });
    }

    protected override void OnUpdate()
    {
        var concurrentECB = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var queueRandom = new Random((uint)(1 + Time.ElapsedTime * 10));

        var trainCarEntities =
            m_TrainCarsQuery.ToEntityArrayAsync(Allocator.TempJob, out var trainCarEntitiesHandle);
        var trainCars =
            m_TrainCarsQuery.ToComponentDataArrayAsync<TrainCar>(Allocator.TempJob, out var trainCarsHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, trainCarEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, trainCarsHandle);
        var seatsBufferFromEntity = GetBufferFromEntity<Seat>();

        Entities
            .WithDeallocateOnJobCompletion(trainCarEntities)
            .WithDeallocateOnJobCompletion(trainCars)
            .ForEach((Entity commuterEntity, int entityInQueryIndex, ref CommuterBoarding commuterBoarding, ref Translation translation, in Commuter commuter) =>
            {
                var queueIndex = commuterBoarding.QueueIndex;
                if (queueIndex < 0)
                {
                    var selectedQueueIndex = queueRandom.NextInt(0, 5);
                    var platform = GetComponent<Platform>(commuter.CurrentPlatform);
                    Entity queueWaypointEntity;
                    switch (selectedQueueIndex)
                    {
                        case 0:
                            queueWaypointEntity = platform.Queue0;
                            break;
                        case 1:
                            queueWaypointEntity = platform.Queue1;
                            break;
                        case 2:
                            queueWaypointEntity = platform.Queue2;
                            break;
                        case 3:
                            queueWaypointEntity = platform.Queue3;
                            break;
                        default:
                            queueWaypointEntity = platform.Queue4;
                            break;
                    }

                    var queueWaypoint = GetComponent<Waypoint>(queueWaypointEntity);
                    commuterBoarding.QueueIndex = selectedQueueIndex;
                    translation.Value = queueWaypoint.WorldPosition;
                }
                else
                {
                    //Debug.Log("commuter boarding");
                    for (var i = 0; i < trainCars.Length; ++i)
                    {
                        var trainCar = trainCars[i];
                        //Debug.Log("car index = " + trainCar.indexInTrain + ", queue index = " + queueIndex);
                        if (trainCar.indexInTrain != queueIndex)
                            continue;

                        var trainState = GetComponent<TrainState>(trainCar.train);
                        //Debug.Log("time til departure = " + trainState.timeUntilDeparture);
                        if (trainState.currentPlatform != commuter.CurrentPlatform || trainState.timeUntilDeparture <= 0f)
                            continue;

                        var boarded = false;
                        var carEntity = trainCarEntities[i];
                        var seatsBuffer = seatsBufferFromEntity[carEntity];
                        //Debug.Log("***looking for seat****");
                        for (var j = 0; j < seatsBuffer.Length; ++j)
                        {
                            var seat = seatsBuffer[j];
                            //Debug.Log("seat " + j + " occupied by " + seat.occupiedBy);
                            if (seat.occupiedBy == Entity.Null)
                            {
                                //Debug.Log("**!!!claimed seat " + j + "!!!**");
                                seat.occupiedBy = commuterEntity;
                                concurrentECB.RemoveComponent<CommuterBoarding>(entityInQueryIndex, commuterEntity);
                                boarded = true;
                                seatsBuffer[j] = seat;
                                break;
                            }
                        }

                        if (boarded)
                            break;
                    }
                }
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
