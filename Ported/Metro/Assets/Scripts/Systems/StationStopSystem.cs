using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class StationStopSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;    

        Entities.ForEach((  ref SplineFollower splineFollower,
                            ref Ticker ticker,
                            ref NextStation nextStation,
                            in TrackProgress trackProgress) =>
        {

            var stationDistanceBuffer = GetBuffer<FloatBufferElement>(splineFollower.track);
            
            var distanceToNextStation =
                math.abs(trackProgress.Value - stationDistanceBuffer[nextStation.stationIndex]);

            if (ticker.TimeRemaining <= 0)
            {
                if (trackProgress.Value >= stationDistanceBuffer[nextStation.stationIndex] && distanceToNextStation < 10)
                {
                    ticker.TimeRemaining = 5f;
                    nextStation.stationIndex += 1;
                    
                    nextStation.stationIndex = nextStation.stationIndex % stationDistanceBuffer.Length;
                }
            }
            else
            {
                ticker.TimeRemaining -= deltaTime;
            }

            

        }).Schedule();
    }
}
