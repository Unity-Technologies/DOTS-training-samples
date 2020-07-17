using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CarUpdateSystem : SystemBase
{
    
    //private EntityQuery _trainQuery;

    protected override void OnCreate()
    {
        /*
        _trainQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TrainPosition>(),
                ComponentType.ReadOnly<TrainState>()
            }
        });
        */
    }


    protected override void OnUpdate()
    {
        /*
        var trainEntities = _trainQuery.ToEntityArrayAsync(Allocator.TempJob, out var trainEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, trainEntitiesHandle);

        var trainPositionAccessor = _trainQuery.ToComponentDataArrayAsync<TrainPosition>(Allocator.TempJob, out var trainPositionAccessorHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, trainPositionAccessorHandle);

        var trainStateAccessor = _trainQuery.ToComponentDataArrayAsync<TrainState>(Allocator.TempJob, out var trainStateAccessorHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, trainStateAccessorHandle);
        */

        
        var trainPositionAccessor = GetComponentDataFromEntity<TrainPosition>(true);
        var trainStateAccessor = GetComponentDataFromEntity<TrainState>(true);
        var trackPointAccessor = GetBufferFromEntity<TrackPoint>(true);

        CarSpacing spacing = GetSingleton<CarSpacing>();

        Entities
            .WithReadOnly(trainPositionAccessor)
            .WithReadOnly(trainStateAccessor)
            .WithReadOnly(trackPointAccessor)
            .ForEach((ref Translation translation, ref Rotation rotation, ref Color colorCmp, in TrainCar trainCar) =>
        {
            

            TrainPosition trainPosition = trainPositionAccessor[trainCar.train];
            TrainState trainState = trainStateAccessor[trainCar.train];
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

            colorCmp.Value = trainState.timeUntilDeparture > 0.0f ? new float4(0, 1, 0, 1) : new float4(252.0f / 255, 183.0f / 255, 22.0f / 255, 1.0f);

        }).ScheduleParallel();

    }
}
