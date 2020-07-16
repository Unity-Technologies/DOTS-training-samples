using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
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
        BufferFromEntity<TrackStation> platformsAccessor = GetBufferFromEntity<TrackStation>(true);
        
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
                //Entity platformEntity = platforms[trainState.nextPlatform].platform;
                float platformPosition = trainState.inbound ? platforms[trainState.nextPlatform].Inbound.EndT : platforms[trainState.nextPlatform].Outbound.EndT;
                platformPosition *= trackPoints.Length;

                if(oldPosition <= platformPosition && platformPosition <= newPosition)
                {
                    
                    if (trainState.inbound)
                        trainState.nextPlatform--;
                    else
                        trainState.nextPlatform++;

                    Debug.Log($"At Station - Next Station {trainState.nextPlatform}");

                    if (trainState.nextPlatform < 0)
                    {
                        Debug.Log("Next Station Inbound");
                        trainState.inbound = false;
                        trainState.nextPlatform = 0;
                    }
                    else if (trainState.nextPlatform >= platforms.Length)
                    {
                        Debug.Log("Next Station Outbound");
                        trainState.inbound = true;
                        trainState.nextPlatform = platforms.Length - 1;
                    }

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
