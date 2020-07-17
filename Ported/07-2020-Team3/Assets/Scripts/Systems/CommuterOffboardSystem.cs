using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CommuterOffboardSystem : SystemBase
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

        var trainCarEntities =
            m_TrainCarsQuery.ToEntityArrayAsync(Allocator.TempJob, out var trainCarEntitiesHandle);
        var trainCars =
            m_TrainCarsQuery.ToComponentDataArrayAsync<TrainCar>(Allocator.TempJob, out var trainCarsHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, trainCarEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, trainCarsHandle);
        var seatsBufferFromEntity = GetBufferFromEntity<Seat>();

        Entities
            .WithName("Metro_CommuterOffboardJob")
            .WithDeallocateOnJobCompletion(trainCarEntities)
            .WithDeallocateOnJobCompletion(trainCars)
            .ForEach((Entity commuterEntity, int entityInQueryIndex, ref Commuter commuter, in Translation translation,
                in DynamicBuffer<CommuterWaypoint> waypointsBuffer, in RidingTrain commuterBoarding) =>
            {
                for (var i = 0; i < trainCars.Length; ++i)
                {
                    var trainCar = trainCars[i];
                    var trainState = GetComponent<TrainState>(trainCar.train);
                    if (trainState.currentPlatform != commuter.NextPlatform || trainState.timeUntilDeparture <= 0f)
                        continue;

                    var offboarded = false;
                    var carEntity = trainCarEntities[i];
                    var seatsBuffer = seatsBufferFromEntity[carEntity];
                    for (var j = 0; j < seatsBuffer.Length; ++j)
                    {
                        var seat = seatsBuffer[j];
                        if (!seat.occupiedBy.Equals(commuterEntity))
                            continue;

                        seat.occupiedBy = Entity.Null;
                        seatsBuffer[j] = seat;

                        concurrentECB.RemoveComponent<RidingTrain>(entityInQueryIndex, commuterEntity);
                        concurrentECB.AddComponent<CommuterWalking>(entityInQueryIndex, commuterEntity);
                        var nextWaypointEntity = waypointsBuffer[commuter.NextWaypoint].Value;
                        var nextWaypointPosition = GetComponent<Waypoint>(nextWaypointEntity).WorldPosition;
                        commuter.Direction = math.normalize(nextWaypointPosition - translation.Value);
                        offboarded = true;
                        break;
                    }

                    if (offboarded)
                        break;
                }
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
