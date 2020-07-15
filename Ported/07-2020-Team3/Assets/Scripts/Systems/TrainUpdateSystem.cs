using System.Collections;
using System.Collections.Generic;
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

        float deltaTime = Time.DeltaTime;

        BufferFromEntity<TrackPoint> trackPointsAccessor = GetBufferFromEntity<TrackPoint>(true);

        Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref TrainPosition trainPosition) =>
        {

            var trackPoints = trackPointsAccessor[trainPosition.track];
            
            trainPosition.position += trainPosition.speed * deltaTime;
            if (trainPosition.position >= trackPoints.Length)
                trainPosition.position = 0.0f;

        }).Schedule();




    }
}
