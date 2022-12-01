using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

// TODO - Move to different file!
readonly partial struct PassengerDecisionAspect : IAspect
{
    public readonly Entity Self;

    //public readonly TransformAspect Transform;
    public readonly RefRW<Passenger> Passenger;
    public readonly DynamicBuffer<Waypoint> Waypoints;
    public readonly RefRW<PlatformId> PlatformId;
}

[WithAll(typeof(Passenger))]
[BurstCompile]
partial struct PassengerDecisionJob : IJobEntity
{
    public BufferLookup<Waypoint> waypointsLookup;
    public Random random;
    public NativeArray<Platform> platforms;
    public NativeArray<StationId> stationIds;

    void Execute(PassengerDecisionAspect passengerDecisionAspect, ref PlatformId platform1)
    {
        if (waypointsLookup.IsBufferEnabled(passengerDecisionAspect.Self))
            return;

        switch (passengerDecisionAspect.Passenger.ValueRO.State)
        {
            case PassengerState.Idle: // Passenger has just arrived at the station
                var currentPlatformId = passengerDecisionAspect.PlatformId.ValueRO.Value;

                // Get Station the Passenger is on
                var currentStationId = stationIds[currentPlatformId].Value;
                Platform currentPlatform = platforms[currentPlatformId];
                // foreach (var (platformId, stationId, platform) in platformsQuery)
                // {
                //     if(platformId.Value != currentPlatformId)
                //         continue;
                //     
                //     currentStationId = stationId.Value;
                //     currentPlatform = platform;
                //     break;
                // }

                // Get other platforms on the same Station
                int otherPlatformsCount = 0;
                var otherPlatformsIds = CollectionHelper.CreateNativeArray<int>(20, Allocator.TempJob);
                // var otherPlatforms = CollectionHelper.CreateNativeArray<Platform>(20, Allocator.TempJob);
                // foreach (var (platformId, stationId, platform) in platformsQuery)
                // {
                //     if(stationId.Value != currentStationId || platformId.Value == currentPlatformId)
                //         continue;
                //     otherPlatformsIds[otherPlatformsCount] = platformId.Value;
                //     otherPlatforms[otherPlatformsCount] = platform;
                //     otherPlatformsCount++;
                // }

                for (var i = 0; i < stationIds.Length; i++)
                {
                    if (stationIds[i].Value != currentStationId)
                        continue;
                    otherPlatformsIds[otherPlatformsCount] = i;
                    otherPlatformsCount++;
                }

                // Pick a random destination Platform
                var randomIndex = random.NextInt(0, otherPlatformsCount);
                var destinationPlatformId = otherPlatformsIds[randomIndex];
                var destinationPlatform = platforms[destinationPlatformId];

                // Set the destination waypoints
                waypointsLookup.SetBufferEnabled(passengerDecisionAspect.Self, true);

                //var platformWaypoints = SystemAPI.GetBuffer<Waypoint>(passengerDecisionAspect.Self);
                passengerDecisionAspect.Waypoints.Add(new Waypoint { Value = currentPlatform.WalkwayBackLower });
                passengerDecisionAspect.Waypoints.Add(new Waypoint { Value = currentPlatform.WalkwayBackUpper });
                passengerDecisionAspect.Waypoints.Add(new Waypoint { Value = destinationPlatform.WalkwayFrontUpper });
                passengerDecisionAspect.Waypoints.Add(new Waypoint { Value = destinationPlatform.WalkwayFrontLower });

                // Updates the component values
                passengerDecisionAspect.Passenger.ValueRW.State = PassengerState.WalkingToPlatform;
                passengerDecisionAspect.PlatformId.ValueRW.Value = destinationPlatformId;
                return;

            case PassengerState.ChoosingQueue:
                waypointsLookup.SetBufferEnabled(passengerDecisionAspect.Self, true);
                var queueWaypoints = SystemAPI.GetBuffer<Waypoint>(passengerDecisionAspect.Self);
                queueWaypoints.Add(new Waypoint { Value = new float3(1, 0, 1) });
                return;


            /*case PassengerState.ReadyToEnterTrain:

                return;

            case PassengerState.ReadyToExitTrain:

                return;*/

            case PassengerState.WalkingToPlatform:
            case PassengerState.WalkingToQueue:
            case PassengerState.InQueue:
            case PassengerState.OnBoarding:
            case PassengerState.OffBoarding:
            case PassengerState.Seated:
                return; // This should have been caught by the lookup, but you never know...
        }

        //TurretActiveFromEntity.SetComponentEnabled(entity, math.lengthsq(transform.Position) > SquaredRadius);
    }
}

[BurstCompile]
//[RequireMatchingQueriesForUpdate]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct PassengerBrainSystem : ISystem
{
    private BufferLookup<Waypoint> _waypointsLookup;
    private Random _random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Platform>();
        state.RequireForUpdate<Passenger>();
        _waypointsLookup = state.GetBufferLookup<Waypoint>();
        _random = new Random(412867401);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //var platformConfig = SystemAPI.GetSingleton<PlatformConfig>();
        _waypointsLookup.Update(ref state);
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // var decisionJob = new PassengerDecisionJob
        // {
        //     waypointsLookup = _waypointsLookup,
        //     random = _random,
        //     platforms = platformConfig.Platforms,
        //     stationIds = platformConfig.StationIds
        // };
        // decisionJob.Schedule();
        
        foreach (var (passenger, passengerPlatformId, passangerTransform, entity) in SystemAPI.Query<Passenger, PlatformId, WorldTransform>().WithEntityAccess())
        {
            if (_waypointsLookup.IsBufferEnabled(entity))
                continue;

            switch (passenger.State)
            {
                case PassengerState.Idle: // Passenger has just arrived at the station
                    var currentPlatformId = passengerPlatformId.Value;

                    // Get Station the Passenger is on
                    var currentStationId = -1;
                    Platform currentPlatform = default;
                    foreach (var (platformId, stationId, platform) in SystemAPI.Query<PlatformId, StationId, Platform>())
                    {
                        if (platformId.Value != currentPlatformId)
                            continue;

                        currentPlatform = platform;
                        currentStationId = stationId.Value;
                        break;
                    }

                    // Get other platforms on the same Station
                    int otherPlatformsCount = 0;
                    var otherPlatformsIds = CollectionHelper.CreateNativeArray<int>(20, Allocator.Temp);
                    var otherPlatforms = CollectionHelper.CreateNativeArray<Platform>(20, Allocator.Temp);
                    foreach (var (platformId, stationId, platform) in SystemAPI.Query<PlatformId, StationId, Platform>())
                    {
                        if (stationId.Value != currentStationId || platformId.Value == currentPlatformId)
                            continue;
                        otherPlatformsIds[otherPlatformsCount] = platformId.Value;
                        otherPlatforms[otherPlatformsCount] = platform;
                        otherPlatformsCount++;
                    }

                    // Pick a random destination Platform
                    var randomIndex = _random.NextInt(0, otherPlatformsCount);
                    var destinationPlatformId = otherPlatformsIds[randomIndex];
                    var destinationPlatform = otherPlatforms[randomIndex];

                    // Set the destination waypoints
                    _waypointsLookup.SetBufferEnabled(entity, true);

                    // Gets the best path by comparing distances
                    float3 closestLower, closestUpper, otherLower, otherUpper;
                    if (math.distancesq(passangerTransform.Position, currentPlatform.WalkwayBackLower) < math.distancesq(passangerTransform.Position, currentPlatform.WalkwayFrontLower))
                    {
                        closestLower = currentPlatform.WalkwayBackLower;
                        closestUpper = currentPlatform.WalkwayBackUpper;
                    }
                    else
                    {
                        closestLower = currentPlatform.WalkwayFrontLower;
                        closestUpper = currentPlatform.WalkwayFrontUpper;
                    }
                    if (math.distancesq(closestUpper, destinationPlatform.WalkwayBackUpper) < math.distancesq(closestUpper, destinationPlatform.WalkwayFrontUpper))
                    {
                        otherLower = destinationPlatform.WalkwayBackLower;
                        otherUpper = destinationPlatform.WalkwayBackUpper;
                    }
                    else
                    {
                        otherLower = destinationPlatform.WalkwayFrontLower;
                        otherUpper = destinationPlatform.WalkwayFrontUpper;
                    }

                    // Sets the series of waypoints
                    var platformWaypoints = SystemAPI.GetBuffer<Waypoint>(entity);
                    platformWaypoints.Add(new Waypoint { Value = closestLower });
                    platformWaypoints.Add(new Waypoint { Value = closestUpper });
                    platformWaypoints.Add(new Waypoint { Value = otherUpper });
                    platformWaypoints.Add(new Waypoint { Value = otherLower });

                    // Updates the component values
                    ecb.SetComponent(entity, new PlatformId { Value = destinationPlatformId });
                    ecb.SetComponent(entity, new Passenger { State = PassengerState.WalkingToPlatform });
                    continue;

                case PassengerState.ChoosingQueue:
                    _waypointsLookup.SetBufferEnabled(entity, true);
                    
                    (DynamicBuffer<PlatformQueueBuffer>, WorldTransform, PlatformQueueId) queueInfo = default;
                    var queueLength = int.MaxValue;
                    foreach (var (platformId, buffer, queueTransform, queueId) in SystemAPI.Query<PlatformId, DynamicBuffer<PlatformQueueBuffer>, WorldTransform, PlatformQueueId>())
                    {
                        if (platformId.Value == passengerPlatformId.Value && buffer.Length < queueLength)
                        {
                            queueInfo = (buffer, queueTransform, queueId);
                            queueLength = buffer.Length;
                        }
                    }

                    var waypoints = SystemAPI.GetBuffer<Waypoint>(entity);
                    waypoints.Add(new Waypoint { Value = _random.NextFloat3(new float3(-1.2f, 0f, -1f), new float3(1.2f, 0f, 1f)) + queueInfo.Item2.Position });
                    queueInfo.Item1.Add(new PlatformQueueBuffer{Passenger = entity});
                    ecb.SetComponent(entity, new Passenger { State = PassengerState.WalkingToQueue });
                    ecb.SetComponent(entity, new PlatformQueueId { Value = queueInfo.Item3.Value});
                    continue;


                /*case PassengerState.ReadyToEnterTrain:

                    continue;

                case PassengerState.ReadyToExitTrain:

                    continue;*/

                case PassengerState.WalkingToPlatform:
                case PassengerState.WalkingToQueue:
                case PassengerState.InQueue:
                case PassengerState.OnBoarding:
                case PassengerState.OffBoarding:
                case PassengerState.Seated:
                    continue; // This should have been caught by the lookup, but you never know...
            }
        }
    }
}