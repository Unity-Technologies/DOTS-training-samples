using System;
using System.Runtime.CompilerServices;
using Metro;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Metro
{
    public partial struct TrainMoverSystem : ISystem
    {
        float3 m_TrainUp;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            m_TrainUp = new float3(0, 1f, 0);

            state.RequireForUpdate<Train>();
            state.RequireForUpdate<Config>();
            state.RequireForUpdate<TrackPoint>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var config = SystemAPI.GetSingleton<Config>();
            float deltaTime = SystemAPI.Time.DeltaTime;

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
                UpdateTrainOrientation(transform, directionToNextPoint);

                if (em.IsComponentEnabled<EnRouteComponent>(entity))
                {
                    float3 position = transform.ValueRO.Position - train.ValueRO.Offset;
                    float currentSpeed = train.ValueRO.Speed;

                    float distanceToNextStop = DistanceToNextStop(position, track, currentIndex, indexDirection);
                    float distanceRequiredToStop = DistanceToStop(currentSpeed, config.TrainAcceleration);
                    bool shouldStop = distanceRequiredToStop >= distanceToNextStop;
                    float acceleration = shouldStop ? -config.TrainAcceleration : config.TrainAcceleration;

                    float speed = math.clamp(train.ValueRO.Speed + (acceleration * deltaTime), config.MinTrainSpeed,
                        config.MaxTrainSpeed);
                    if (em.IsComponentEnabled<DepartingComponent>(entity) &&
                        Math.Abs(speed - config.MaxTrainSpeed) < float.Epsilon)
                        em.SetComponentEnabled<DepartingComponent>(entity, false);

                    if (!em.IsComponentEnabled<ArrivingComponent>(entity) && shouldStop)
                        em.SetComponentEnabled<ArrivingComponent>(entity, true);

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
                                ArriveAtStation(ref state, entity, train, nextPoint, em);
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

                        train.ValueRW.Speed = speed;
                    }

                    train.ValueRW.Forward = forward;
                    train.ValueRW.TrackPointIndex = currentIndex;
                    var finalPos = position + train.ValueRW.Offset;
                    transform.ValueRW.Position = finalPos;
                }

                else if (em.IsComponentEnabled<UnloadingComponent>(entity))
                    UpdateUnloading(entity, train, config, em, deltaTime);

                else if (em.IsComponentEnabled<LoadingComponent>(entity))
                    UpdateLoading(ref state, entity, train, config, em, deltaTime);
            }
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ArriveAtStation(ref SystemState state, Entity trainEntity, RefRW<Train> train, TrackPoint trackPoint,
            EntityManager em)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var children = em.GetBuffer<LinkedEntityGroup>(trainEntity);
            foreach (var child in children)
            {
                if (em.HasComponent<UnloadingComponent>(child.Value))
                {
                    ecb.SetComponentEnabled<UnloadingComponent>(child.Value, true);
                }
            }

            train.ValueRW.Duration = 0;
            train.ValueRW.Speed = 0;
            train.ValueRW.StationEntity = trackPoint.Station;
            em.SetComponentEnabled<UnloadingComponent>(trainEntity, true);
            em.SetComponentEnabled<EnRouteComponent>(trainEntity, false);
            em.SetComponentEnabled<ArrivingComponent>(trainEntity, false);
            Debug.Log($"Travel complete: arrived at station {train.ValueRO.StationEntity}");
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UpdateTrainOrientation(RefRW<LocalTransform> transform, float3 direction)
        {
            transform.ValueRW.Rotation = quaternion.LookRotation(direction, m_TrainUp);
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UpdateLoading(ref SystemState state, Entity trainEntity, RefRW<Train> train, Config config,
            EntityManager em, float deltaTime)
        {
            train.ValueRW.Duration += deltaTime;
            if (train.ValueRW.Duration >= config.UnloadingTime)
            {
                Debug.Log(
                    $"Loading complete at station {train.ValueRO.StationEntity}, Setting movement state to EnRoute");
                train.ValueRW.Duration = 0;
                train.ValueRW.StationEntity = Entity.Null;
                em.SetComponentEnabled<LoadingComponent>(trainEntity, false);
                em.SetComponentEnabled<EnRouteComponent>(trainEntity, true);
                em.SetComponentEnabled<DepartingComponent>(trainEntity, true);

                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                var children = em.GetBuffer<LinkedEntityGroup>(trainEntity);
                foreach (var child in children)
                {
                    if (em.HasComponent<DepartingComponent>(child.Value))
                    {
                        ecb.SetComponentEnabled<DepartingComponent>(child.Value, true);
                    }
                }
            }
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UpdateUnloading(Entity trainEntity, RefRW<Train> train, Config config, EntityManager em, float deltaTime)
        {
            train.ValueRW.Duration += deltaTime;
            if (train.ValueRW.Duration >= config.UnloadingTime)
            {
                Debug.Log(
                    $"Unloading complete at station {train.ValueRO.StationEntity}.  Setting movement state to Loading");
                train.ValueRW.Duration = 0;
                em.SetComponentEnabled<UnloadingComponent>(trainEntity, false);
                em.SetComponentEnabled<LoadingComponent>(trainEntity, true);
                em.SetComponentEnabled<ArrivingComponent>(trainEntity, false);
            }
        }

        [BurstCompile, MethodImpl(MethodImplOptions.AggressiveInlining)]
        float DistanceToStop(float speed, float deceleration)
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
        float DistanceToNextStop(float3 trainPosition, DynamicBuffer<TrackPoint> track, int currentIndex,
            int indexDirection)
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
    }
}
