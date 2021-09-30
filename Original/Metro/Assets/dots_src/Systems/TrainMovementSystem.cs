using System;
using System.Data.Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TrainMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settings = GetSingleton<Settings>();
        var deltaTime = Time.DeltaTime;
        
        // update state of train, i.e. stopping, starting, etc
        // this way we can access position of train in front, as we're not allowed to access `ref` components of other entities
        Entities.ForEach((ref Translation translation, ref TrainMovement movement, in LineIndex lineIndex, in TrainInFront trainInFront, in TrainPosition position) =>
        {
            ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];
            
            switch (movement.state)
            {
                case TrainMovemementStates.Starting:
                    movement.speed += settings.MaxSpeed*settings.Acceleration*deltaTime;
                    if (movement.speed >= settings.MaxSpeed)
                        movement.state = TrainMovemementStates.Running;
                    break;
                
                case TrainMovemementStates.Stopping:
                    var unitPointDistance = splineBlobAsset.UnitPointDistanceToClosestPlatform(position.position);
                    if (unitPointDistance > movement.distanceToStop) // Keep running if the platform was passed while transitioning to Stopped
                    {
                        movement.state = TrainMovemementStates.Running;
                        break;
                    }
                    movement.speed = (unitPointDistance / movement.distanceToStop) * settings.MaxSpeed;

                    if (movement.speed <= 0.0f || unitPointDistance < .1f)
                    {
                        movement.speed = 0.0f;
                        movement.restingTimeLeft = settings.TimeAtStation;
                        movement.state = TrainMovemementStates.Waiting;
                    }
                    break;
                
                case TrainMovemementStates.Waiting:
                    if (movement.restingTimeLeft < 0)
                        movement.state = TrainMovemementStates.Starting;
                    else 
                        movement.restingTimeLeft -= deltaTime;
                    
                    break;

                case TrainMovemementStates.Running:
                    // are we approaching train in front?
                    float unitPointsToTrainInFront;
                    var trainInFrontPosition = GetComponent<TrainPosition>(trainInFront.Train).position;

                    if (position.position < trainInFrontPosition)
                    {
                        unitPointsToTrainInFront = trainInFrontPosition - position.position;    
                    }
                    else
                    {
                        // train in front has "overflown" train line array
                        unitPointsToTrainInFront = splineBlobAsset.equalDistantPoints.Length - position.position + trainInFrontPosition;
                    }

                    var unitPointBreakingDistance = splineBlobAsset.DistanceToPointUnitDistance(settings.TrainBreakingDistance);
                    if (unitPointsToTrainInFront < unitPointBreakingDistance)
                    {
                        movement.distanceToStop = unitPointsToTrainInFront;
                        movement.state = TrainMovemementStates.Stopping;
                        break;
                    }
                    
                    // are we approaching platform?
                    var unitPointDistToClosestPlatform = splineBlobAsset.UnitPointDistanceToClosestPlatform(position.position);
                    unitPointBreakingDistance = splineBlobAsset.DistanceToPointUnitDistance(settings.PlatformBreakingDistance);
                    if (unitPointDistToClosestPlatform < unitPointBreakingDistance)
                    {
                        movement.distanceToStop = unitPointDistToClosestPlatform;
                        movement.state = TrainMovemementStates.Stopping;
                    }
                    break;
            }
        }).ScheduleParallel();

        // update train positions based on current speed
        Entities.ForEach((ref TrainPosition position, ref Translation translation, in LineIndex lineIndex, in TrainMovement movement) =>
        {
            ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];

            position.position += splineBlobAsset.DistanceToPointUnitDistance(movement.speed * deltaTime);
            position.position = math.fmod(position.position, splineBlobAsset.equalDistantPoints.Length);
            (translation.Value, _) = splineBlobAsset.PointUnitPosToWorldPos(position.position);
        }).ScheduleParallel();
    }
}

public static class UnitConvertExtensionMethods
{
    public static (float3, quaternion) PointUnitPosToWorldPos(this ref SplineBlobAsset splineBlob, float unitPointPos)
    {
        
        var floor = (int)math.floor(unitPointPos);
        var ceil = (floor+1) % splineBlob.equalDistantPoints.Length;

        var p1 = splineBlob.equalDistantPoints[floor];
        var p2= splineBlob.equalDistantPoints[ceil];

        return (math.lerp(splineBlob.equalDistantPoints[floor], splineBlob.equalDistantPoints[ceil], math.frac(unitPointPos)), 
            quaternion.LookRotation(splineBlob.equalDistantPoints[floor] - splineBlob.equalDistantPoints[ceil], math.up()));
    }
    
    /// <summary>
    /// Returns distance to next platform in unit point distance, i.e. range [0..number_of_points[ 
    /// </summary>
    public static float UnitPointDistanceToClosestPlatform(this ref SplineBlobAsset splineBlob, float unitPointPos)
    {
        unitPointPos %= splineBlob.equalDistantPoints.Length;
        for (var i = 0; i < splineBlob.unitPointPlatformPositions.Length; i++)
        {
            var platformPosition = splineBlob.unitPointPlatformPositions[i];
            if (unitPointPos < splineBlob.unitPointPlatformPositions[i]) return splineBlob.unitPointPlatformPositions[i] - unitPointPos;
        }

        return splineBlob.unitPointPlatformPositions[0] + (splineBlob.equalDistantPoints.Length - unitPointPos);
    }
    
    public static float DistanceToPointUnitDistance(this ref SplineBlobAsset splineBlob, float distance)
    {
        return distance / splineBlob.length * splineBlob.equalDistantPoints.Length;
    }
}
