using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class TrainUpdateSystem : SystemBase
{
    private EntityQuery _trackQuery;

    protected override void OnCreate()
    {
        _trackQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TrackPoint>()
            }
        });
    }

    protected override void OnUpdate()
    {

        if (!HasSingleton<TrainWaitTime>())
            return;

        if (!HasSingleton<MaximumTrainSpeed>())
            return;

        float deltaTime = Time.DeltaTime;

        BufferFromEntity<TrackPoint> trackPointsAccessor = GetBufferFromEntity<TrackPoint>(true);
        BufferFromEntity<TrackPlatforms> platformsAccessor = GetBufferFromEntity<TrackPlatforms>(true);
        
        MaximumTrainSpeed maximumTrainSpeed = GetSingleton<MaximumTrainSpeed>();
        TrainWaitTime trainWaitTime = GetSingleton<TrainWaitTime>();

        float dt = Time.DeltaTime;

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref TrainPosition trainPosition, ref TrainState trainState) =>
        {

            trainState.timeUntilDeparture -= dt;

            if (trainState.timeUntilDeparture < 0)
            {
                var trackPoints = trackPointsAccessor[trainPosition.track];
                var platforms = platformsAccessor[trainPosition.track];

                trainPosition.speed = maximumTrainSpeed.Value;

                float oldPosition = trainPosition.position;
                float newPosition = oldPosition + trainPosition.speed * deltaTime;
                Entity platformEntity = platforms[trainState.nextPlatform].platform;
                float platformPosition = platforms[trainState.nextPlatform].position;

                if(oldPosition <= platformPosition && platformPosition <= newPosition)
                {
                    trainState.currentPlatform = platformEntity;
                    trainState.nextPlatform++;
                    if (trainState.nextPlatform >= platforms.Length)
                        trainState.nextPlatform = 0;

                    trainState.timeUntilDeparture = trainWaitTime.Value;
                }


                trainPosition.position = newPosition;
                if (trainPosition.position >= trackPoints.Length)
                    trainPosition.position = 0.0f;
            }
            else
            {
                trainState.currentPlatform = Entity.Null;
            }

        }).Schedule();




    }
}
