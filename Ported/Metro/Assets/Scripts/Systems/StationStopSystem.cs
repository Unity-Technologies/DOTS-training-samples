using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SplineFollowerSystem))]
public partial class StationStopSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;    

        Entities
            //.WithoutBurst() // Uncomment to debug the body of the Foreach
            .ForEach((  ref SplineFollower splineFollower,
                            ref Ticker ticker,
                            ref CurrentRoute currentRoute,
                            ref TrackProgress trackProgress,
                            in Spline spline) =>
        {
            ref var splineData = ref spline.splinePath.Value;
            
            switch (currentRoute.state)
            {
                case TrainState.Stopped:
                    ticker.TimeRemaining -= deltaTime;
                    if (ticker.TimeRemaining <= 0)
                    {
                        var bufferFromEntity = GetBufferFromEntity<FloatBufferElement>(true);
                        var stationDistanceBuffer = bufferFromEntity[splineFollower.track];
                        var newStationIndex = (currentRoute.targetStationIndex + 1) % stationDistanceBuffer.Length;
                        
                        float startLocation = stationDistanceBuffer[currentRoute.targetStationIndex];
                        float endLocation = stationDistanceBuffer[newStationIndex];
                        if (startLocation>endLocation) startLocation -= splineData.pathLength;
                        if (trackProgress.Value > endLocation) trackProgress.Value -= splineData.pathLength;
                        
                        currentRoute.targetStationIndex = newStationIndex;
                        currentRoute.routeStartLocation = startLocation;
                        currentRoute.routeEndLocation = endLocation;
                        currentRoute.state = TrainState.Departing;
                    }
                    break;
                case TrainState.Approaching:
                    {
                        if (trackProgress.Value >= currentRoute.routeEndLocation)
                        {
                            currentRoute.state = TrainState.Stopped;
                            ticker.TimeRemaining = 5.0f;
                        }
                    }
                    break;
                case TrainState.Departing:
                    {
                        if (trackProgress.Value > currentRoute.routeStartLocation + 20)
                        {
                            currentRoute.state = TrainState.InTransit;
                        }
                    }
                    break;
                case TrainState.InTransit:
                    if (trackProgress.Value > currentRoute.routeEndLocation - 20)
                    {
                        currentRoute.state = TrainState.Approaching;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }).ScheduleParallel();
    }
}
