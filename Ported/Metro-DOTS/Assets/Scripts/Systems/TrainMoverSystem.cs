using System;
using Components;
using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct TrainMoverSystem  : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Train>();
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrackPoint>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    public float DistanceToStop(float speed, float deceleration)
    {
        float distance = 0;
        while (speed > 0)
        {
            distance += speed;
            speed -= deceleration;
        }
        return distance;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        var config = SystemAPI.GetSingleton<Config>();

        foreach (var (transform, train, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<Train>>()
                     .WithEntityAccess())
        {
            var track = em.GetBuffer<TrackPoint>(train.ValueRO.TrackEntity);
            bool forward = train.ValueRO.Forward;
            int currentIndex = train.ValueRO.TrackPointIndex;
            int indexDirection = forward ? 1 : -1;
            int nextIndex = currentIndex + indexDirection;
                
            TrackPoint currentPoint = track[currentIndex];
            TrackPoint nextPoint = track[nextIndex];

            float3 currentPosition = currentPoint.Position;
            float3 nextPosition = nextPoint.Position;
            
            float3 directionToNextPoint = math.normalize(nextPosition - currentPosition);
            transform.ValueRW.Rotation = quaternion.LookRotation(new float3(1f, 0, 0), new float3(0, 1f, 0));
            
            if (em.IsComponentEnabled<EnRouteComponent>(entity))
            {
                float3 position = transform.ValueRO.Position - train.ValueRO.Offset;
                
                float distanceToNextPoint = math.distance(nextPosition, position);

                float distanceToStop = distanceToNextPoint;
                int stopIndex = nextIndex;
                TrackPoint stopPoint = track[stopIndex];
                while (!stopPoint.IsEnd || !stopPoint.IsStation)
                {
                    stopIndex += indexDirection;
                    var nextStopPoint = track[stopIndex];
                    stopPoint = track[stopIndex];
                    var pointDistance = math.distance(stopPoint.Position, nextStopPoint.Position);
                    distanceToStop += pointDistance;
                    stopPoint = nextStopPoint;
                }

                float currentSpeed = train.ValueRO.Speed;
                float distanceRequiredToStop = DistanceToStop(currentSpeed, config.TrainAcceleration);
                
                float speed = math.clamp(train.ValueRO.Speed + (config.TrainAcceleration * SystemAPI.Time.DeltaTime), 0, config.MaxTrainSpeed);
                // bool departing = Math.Abs(speed - config.MaxTrainSpeed) < float.Epsilon;
                // if (departing != em.IsComponentEnabled<DepartingComponent>(entity))
                //     em.SetComponentEnabled<DepartingComponent>(entity, departing);

                float totalDistanceToTravel = speed * SystemAPI.Time.DeltaTime;
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
                            train.ValueRW.Duration = 0;
                            train.ValueRW.Speed = 0;
                            train.ValueRW.StationEntity = nextPoint.Station;
                            em.SetComponentEnabled<UnloadingComponent>(entity, true);
                            em.SetComponentEnabled<EnRouteComponent>(entity, false);
                            Debug.Log($"Travel complete: arrived at station {train.ValueRO.StationEntity}");
                            break;
                        }
                    }
                    else
                    {
                        float3 movement = directionToNextPoint * totalDistanceToTravel;
                        var newPos = position + movement;
                        position = newPos;
                        totalDistanceToTravel = 0;
                    }

                    train.ValueRW.Speed = speed;
                }

                train.ValueRW.Forward = forward;
                train.ValueRW.TrackPointIndex = currentIndex;
                var finalPos = position + train.ValueRW.Offset;
                transform.ValueRW.Position = finalPos;
            }
            
            else if (em.IsComponentEnabled<UnloadingComponent>(entity))
            {
                train.ValueRW.Duration += SystemAPI.Time.DeltaTime;
                if (train.ValueRW.Duration >= config.UnloadingTime)
                {
                    Debug.Log($"Unloading complete at station {train.ValueRO.StationEntity}.  Setting movement state to Loading");
                    train.ValueRW.Duration = 0;
                    em.SetComponentEnabled<UnloadingComponent>(entity, false);
                    em.SetComponentEnabled<LoadingComponent>(entity, true);
                }
            }
            
            else if (em.IsComponentEnabled<LoadingComponent>(entity))
            {
                train.ValueRW.Duration += SystemAPI.Time.DeltaTime;
                if (train.ValueRW.Duration >= config.UnloadingTime)
                {
                    Debug.Log($"Loading complete at station {train.ValueRO.StationEntity}, Setting movement state to EnRoute");
                    train.ValueRW.Duration = 0;
                    train.ValueRW.StationEntity = Entity.Null;
                    em.SetComponentEnabled<LoadingComponent>(entity, false);
                    em.SetComponentEnabled<EnRouteComponent>(entity, true);
                }
            }
        }
    }
}
