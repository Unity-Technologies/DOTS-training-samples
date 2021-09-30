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
        
        Entities.ForEach((ref Translation translation, ref TrainMovement movement, in LineEntity lineIndex) =>
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
                    // following are in meters 
                    var unitPointDistance = splineBlobAsset.UnitPointDistanceToClosestPlatform(movement.position);
                    movement.speed = (unitPointDistance / movement.distanceToStation) * settings.MaxSpeed;

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
                    // are we approaching platform?
                    var unitPointDistToClosestPlatform = splineBlobAsset.UnitPointDistanceToClosestPlatform(movement.position);
                    if (unitPointDistToClosestPlatform < splineBlobAsset.DistanceToPointUnitDistance(settings.BreakingDistance))
                    {
                        movement.distanceToStation = unitPointDistToClosestPlatform;
                        movement.state = TrainMovemementStates.Stopping;
                    }

                    break;
            }
            
            movement.position += splineBlobAsset.DistanceToPointUnitDistance(movement.speed * deltaTime);
            movement.position = math.fmod(movement.position, splineBlobAsset.equalDistantPoints.Length);
            (translation.Value, _) = splineBlobAsset.PointUnitPosToWorldPos(movement.position);
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
