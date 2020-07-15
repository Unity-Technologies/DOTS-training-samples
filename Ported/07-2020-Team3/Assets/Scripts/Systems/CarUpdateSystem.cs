using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CarUpdateSystem : SystemBase
{


    protected override void OnUpdate()
    {

        var trainPositionAccessor = GetComponentDataFromEntity<TrainPosition>();
        var trackPointAccessor = GetBufferFromEntity<TrackPoint>();

        CarSpacing spacing = GetSingleton<CarSpacing>();

        Entities.ForEach((ref Translation translation, ref Rotation rotation, in TrainCar trainCar) =>
        {

            TrainPosition trainPosition = trainPositionAccessor[trainCar.train];
            var trackPoints = trackPointAccessor[trainPosition.track];

            int trackIndex = (int)math.floor(trainPosition.position);
            float3 posA = trackPoints[trackIndex].position;
            float3 posB = trackPoints[(trackIndex + 1)%trackPoints.Length].position;
            float3 pos = math.lerp(posA, posB, math.frac(trainPosition.position));

            translation.Value = pos - new float3(spacing.Value * trainCar.indexInTrain);

        }).Schedule();

    }
}
