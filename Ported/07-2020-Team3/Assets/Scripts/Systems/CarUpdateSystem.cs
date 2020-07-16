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

            float carPosition = trainPosition.position - trainCar.indexInTrain * spacing.Value;
            while (carPosition < 0.0f)
                carPosition += trackPoints.Length;

            int trackIndex = (int)math.floor(carPosition);
            float3 posA = trackPoints[trackIndex % trackPoints.Length].position;
            float3 posB = trackPoints[(trackIndex + 1)%trackPoints.Length].position;
            float3 pos = math.lerp(posA, posB, math.frac(carPosition));

            rotation.Value = quaternion.LookRotationSafe(posA - posB, new float3(0, 1, 0));

            translation.Value = pos;

        }).Schedule();

    }
}
