using System;
using System.Runtime.CompilerServices;
using Metro;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile, WithAll(typeof(LocalTransform), typeof(Train))]
public partial struct TrainMoverJob : IJobEntity
{
    public Config config;
    public float deltaTime;

    public BufferLookup<LinkedEntityGroup> childBufferLookup;
    public BufferLookup<TrackPoint> trackBufferLookup;
    public ComponentLookup<EnRouteComponent> enrouteLookup;
    public ComponentLookup<DepartingComponent> departingLookup;
    public ComponentLookup<ArrivingComponent> arrivingLookup;
    public ComponentLookup<UnloadingComponent> unloadingLookup;
    public ComponentLookup<LoadingComponent> loadingLookup;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref LocalTransform transform, ref Train train)
    {
        trackBufferLookup.TryGetBuffer(train.TrackEntity, out var track);
        bool forward = train.Forward;
        
        int currentIndex = train.TrackPointIndex;
        int indexDirection = forward ? 1 : -1;
        int nextIndex = currentIndex + indexDirection;

        TrackPoint currentPoint = track[currentIndex];
        TrackPoint nextPoint = track[nextIndex];

        float3 currentPosition = currentPoint.Position;
        float3 nextPosition = nextPoint.Position;

        float3 directionToNextPoint = math.normalize(nextPosition - currentPosition);
        transform.Rotation = quaternion.LookRotation(directionToNextPoint, new float3(0, 1f, 0));
        
        if (enrouteLookup.IsComponentEnabled(entity))
        {
            float3 position = transform.Position - train.Offset;
            float currentSpeed = train.Speed;

            float distanceToNextStop = DistanceToNextStop(ref position, ref track, currentIndex, indexDirection);
            float distanceRequiredToStop = DistanceToStop(currentSpeed, config.TrainAcceleration);
            bool shouldStop = distanceRequiredToStop >= distanceToNextStop;
            float acceleration = shouldStop ? -config.TrainAcceleration : config.TrainAcceleration;

            float speed = math.clamp(train.Speed + (acceleration * deltaTime), config.MinTrainSpeed,
                config.MaxTrainSpeed);
            if (departingLookup.IsComponentEnabled(entity) &&
                Math.Abs(speed - config.MaxTrainSpeed) < float.Epsilon)
                departingLookup.SetComponentEnabled(entity, false);

            if (!arrivingLookup.IsComponentEnabled(entity) && shouldStop)
                arrivingLookup.SetComponentEnabled(entity, true);

            float distanceToNextPoint = math.distance(nextPosition, position);
            float totalDistanceToTravel = speed * deltaTime;
            while (totalDistanceToTravel > 0)
            {
                if (totalDistanceToTravel >= distanceToNextPoint)
                {
                    totalDistanceToTravel -= distanceToNextPoint;
                    position = nextPosition;

                    currentIndex = nextIndex;

                    if (nextPoint.IsEnd)
                        forward = !forward;

                    if (nextPoint.IsStation)
                    {
                        ArriveAtStation(chunkIndex, entity, ref train, nextPoint);
                        totalDistanceToTravel = 0;
                    }
                }
                else
                {
                    float3 movement = directionToNextPoint * totalDistanceToTravel;
                    var newPos = position + movement;
                    position = newPos;

                    totalDistanceToTravel = 0;
                }

                train.Speed = speed;
            }

            train.Forward = forward;
            train.TrackPointIndex = currentIndex;
            var finalPos = position + train.Offset;
            transform.Position = finalPos;
        }

        else if (unloadingLookup.IsComponentEnabled(entity))
            UpdateUnloading(entity, ref train);

        else if (loadingLookup.IsComponentEnabled(entity))
            UpdateLoading(entity, ref train);
        
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float DistanceToNextStop(ref float3 trainPosition, ref DynamicBuffer<TrackPoint> track, int currentIndex, int indexDirection)
    {
        int nextIndex = currentIndex + indexDirection;
        TrackPoint nextPoint = track[nextIndex];
        float3 nextPosition = nextPoint.Position;

        float distanceToNextStop = math.distance(nextPosition, trainPosition);
        int stopIndex = nextIndex;
        TrackPoint stopPoint = track[stopIndex];
        while (!stopPoint.IsStation)
        {
            stopIndex += indexDirection;
            var nextStopPoint = track[stopIndex];
            var pointDistance = math.distance(stopPoint.Position, nextStopPoint.Position);
            distanceToNextStop += pointDistance;
            stopPoint = nextStopPoint;
        }

        return distanceToNextStop;
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float DistanceToStop(float speed, float deceleration)
    {
        float distance = 0;
        while (speed > 0)
        {
            distance += speed;
            speed -= deceleration;
        }

        return distance;
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ArriveAtStation(int sortKey, Entity trainEntity, ref Train train, TrackPoint trackPoint)
    {
        train.Duration = 0;
        train.Speed = 0;
        train.StationEntity = trackPoint.Station;
        unloadingLookup.SetComponentEnabled(trainEntity, true);
        enrouteLookup.SetComponentEnabled(trainEntity, false);
        arrivingLookup.SetComponentEnabled(trainEntity, false);

        childBufferLookup.TryGetBuffer(trainEntity, out var children);
        foreach (var child in children)
        {
            if (unloadingLookup.HasComponent(child.Value))
            {
                unloadingLookup.SetComponentEnabled(child.Value, true);
            }
        }
    }
    
    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateLoading(Entity trainEntity, ref Train train)
    {
        train.Duration += deltaTime;
        if (train.Duration >= config.UnloadingTime)
        {
            train.Duration = 0;
            train.StationEntity = Entity.Null;
            loadingLookup.SetComponentEnabled(trainEntity, false);
            enrouteLookup.SetComponentEnabled(trainEntity, true);
            departingLookup.SetComponentEnabled(trainEntity, true);
        }
        
        childBufferLookup.TryGetBuffer(trainEntity, out var children);
        foreach (var child in children)
        {
            if (departingLookup.HasComponent(child.Value))
            {
                departingLookup.SetComponentEnabled(child.Value, true);
            }
        }
    }

    [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
    void UpdateUnloading(Entity trainEntity, ref Train train)
    {
        train.Duration += deltaTime;
        if (train.Duration >= config.UnloadingTime)
        {
            train.Duration = 0;
            unloadingLookup.SetComponentEnabled(trainEntity, false);
            loadingLookup.SetComponentEnabled(trainEntity, true);
            arrivingLookup.SetComponentEnabled(trainEntity, false);
        }
    }
}