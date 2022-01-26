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

        Entities.ForEach((ref Translation translation,ref SplineFollower splineFollower,ref Ticker ticker,ref NextStation nextStation,in TrackProgress trackProgress, in Rotation rotation) =>
        {

            var track = GetComponent<StationDistanceArray>(splineFollower.track);
            ref var stationDistanceData = ref track.StationDistances.Value;
            var distanceToNextStation =
                math.abs(trackProgress.Value - stationDistanceData.Distances[nextStation.stationIndex]);

            if (ticker.TimeRemaining <= 0)
            {
                if (trackProgress.Value >= stationDistanceData.Distances[nextStation.stationIndex] && distanceToNextStation < 10)
                {
                    ticker.TimeRemaining = 5f;
                    nextStation.stationIndex += 1;
                    
                    nextStation.stationIndex = nextStation.stationIndex % stationDistanceData.Distances.Length;
                }
            }
            else
            {
                ticker.TimeRemaining -= deltaTime;
            }

            

        }).Schedule();
    }
}
