using Components;
using Metro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct TrainMoverSystem  : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TrackPoint>();
    }

    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        var track = SystemAPI.GetSingletonBuffer<TrackPoint>();
        var config = SystemAPI.GetSingleton<Config>();

        foreach (var (transform, train, entity) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<TrainIDComponent>>()
                     .WithEntityAccess())
        {
            if (em.IsComponentEnabled<EnRouteComponent>(entity))
            {
                float3 position = transform.ValueRO.Position - train.ValueRO.Offset;
                int currentIndex = train.ValueRO.TrackPointIndex;
                float totalDistance = config.MaxTrainSpeed * SystemAPI.Time.DeltaTime;
                bool forward = train.ValueRO.Forward;

                while (totalDistance > 0)
                {
                    int nextIndex = forward ? currentIndex + 1 : currentIndex - 1;

                    TrackPoint currentPoint = track[currentIndex];
                    TrackPoint nextPoint = track[nextIndex];
                    float3 currentPosition = currentPoint.Position;
                    float3 nextPosition = nextPoint.Position;
                    
                    float distanceToNextPoint = math.distance(nextPosition, position);
                    float3 directionToNextPoint = math.normalize(nextPosition - currentPosition);

                    transform.ValueRW.Rotation = quaternion.LookRotation(new float3(1f, 0, 0), new float3(0, 1f, 0));

                    if (totalDistance >= distanceToNextPoint)
                    {
                        totalDistance -= distanceToNextPoint;
                        position = nextPosition;

                        currentIndex = nextIndex;

                        if (nextPoint.IsEnd)
                            forward = !forward;
                        
                        if (nextPoint.IsStation)
                        {
                            em.SetComponentEnabled<LoadingComponent>(entity, true);
                            em.SetComponentEnabled<EnRouteComponent>(entity, false);
                            train.ValueRW.Duration = 0;
                            break;
                        }
                    }
                    else
                    {
                        float3 movement = directionToNextPoint * totalDistance;
                        var newPos = position + movement;
                        position = newPos;
                        totalDistance = 0;
                    }
                }

                train.ValueRW.Forward = forward;
                train.ValueRW.TrackPointIndex = currentIndex;
                transform.ValueRW.Position = position + train.ValueRW.Offset;
            }
            
            else if (em.IsComponentEnabled<LoadingComponent>(entity))
            {
                train.ValueRW.Duration += SystemAPI.Time.DeltaTime;
                if (train.ValueRW.Duration >= 2.0f)
                {
                    em.SetComponentEnabled<LoadingComponent>(entity, false);
                    em.SetComponentEnabled<EnRouteComponent>(entity, true);
                }
            }
        }
    }
}
