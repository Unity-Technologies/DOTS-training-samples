using System;
using System.Data.Common;
using dots_src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class TrainMovementSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_SimulationECBSystem;
    protected override void OnCreate()
    {
        m_SimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var splineData = GetSingleton<SplineDataReference>().BlobAssetReference;
        var settingsRef = GetSingleton<Settings>().SettingsBlobRef;
        var deltaTime = Time.DeltaTime;

        var lineEntities = GetEntityQuery(ComponentType.ReadOnly<EntityBufferElement>()).ToEntityArray(Allocator.TempJob);
        var lookup = GetBufferFromEntity<EntityBufferElement>(true);
        var ecb = m_SimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

        // update state of train, i.e. stopping, starting, etc
        // this way we can access position of train in front, as we're not allowed to access `ref` components of other entities
        Entities.WithReadOnly(lookup).WithDisposeOnCompletion(lineEntities)
            .ForEach((Entity e, int entityInQueryIndex, ref TrainMovement movement, ref TrainState state,
                in LineIndex lineIndex, in TrainInFront trainInFront, in TrainPosition position) =>
            {
                ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];
                ref var settings = ref settingsRef.Value;
                if (state.IsMoving)
                {
                    var trainInFrontPosition = GetComponent<TrainPosition>(trainInFront.Train).Value;
                    var unitPointsToTrainInFront = splineBlobAsset.UnitPointDistance(position.Value, trainInFrontPosition);

                    // when calculating stop position, include length of train
                    var unitPointDistanceToEndOfTrainInFront = unitPointsToTrainInFront - splineBlobAsset.GetSizeOfTrainInUnitPoint(ref settings);
                    //Debug.Log($"T{e},{unitPointDistanceToEndOfTrainInFront}");
                    if (unitPointDistanceToEndOfTrainInFront < splineBlobAsset.DistanceToPointUnitDistance(settings.TrainBrakingDistance))
                    {
                        movement.stopPosition = (position.Value + unitPointDistanceToEndOfTrainInFront) % splineBlobAsset.equalDistantPoints.Length;
                        movement.distanceToStop = splineBlobAsset.UnitPointDistance(position.Value, movement.stopPosition);
                        movement.WaitingForState = TrainMovementStates.WaitingBehindTrain;
                        state.State = TrainMovementStates.Stopping;
                    }

                    // are we approaching platform?
                    var unitPointDistToClosestPlatform = splineBlobAsset.UnitPointDistanceToClosestPlatform(position.Value);
                    //Debug.Log($"P{e},{unitPointDistToClosestPlatform}");
                    if (unitPointDistToClosestPlatform < splineBlobAsset.DistanceToPointUnitDistance(settings.PlatformBrakingDistance))
                    {
                        movement.stopPosition = (position.Value + unitPointDistToClosestPlatform) % splineBlobAsset.equalDistantPoints.Length;
                        movement.distanceToStop = unitPointDistToClosestPlatform;
                        movement.WaitingForState = TrainMovementStates.WaitingAtPlatform;
                        state.State = TrainMovementStates.Stopping;
                    }
                }

                switch (state.State)
                {
                    case TrainMovementStates.Starting:
                        movement.speed += settings.MaxSpeed * settings.Acceleration * deltaTime;
                        if (movement.speed >= settings.MaxSpeed)
                            state.State = TrainMovementStates.Running;
                        break;

                    case TrainMovementStates.Stopping:
                        var upToTrainDistance = splineBlobAsset.UnitPointDistance(position.Value, movement.stopPosition);
                        if (upToTrainDistance > movement.distanceToStop) // Keep running if the platform was passed while transitioning to Stopped
                        {
                            state.State = TrainMovementStates.Running;
                            break;
                        }

                        movement.speed = (upToTrainDistance / movement.distanceToStop) * settings.MaxSpeed;

                        if (movement.speed <= 0.0f || upToTrainDistance < .1f)
                        {
                            movement.speed = 0.0f;
                            state.State = movement.WaitingForState; // either waiting at platform or behind another train

                            if (state.State == TrainMovementStates.WaitingAtPlatform)
                            {
                                var lineEntity = lineEntities[lineIndex.Index];
                                var platformEntities = lookup[lineEntity];
                                var platformEntity = splineBlobAsset.GetNextPlatformEntity(ref platformEntities, upToTrainDistance);
                                ecb.SetComponent(entityInQueryIndex, platformEntity, new Occupancy {Train = e, TimeLeft = settings.TimeAtStation});
                            }
                        }
                        break;
                    
                    case TrainMovementStates.WaitingBehindTrain:
                        // TODO: Duplicate code, let's clean up Friday if time
                        var trainInFrontPosition = GetComponent<TrainPosition>(trainInFront.Train).Value;
                        var unitPointsToTrainInFront = splineBlobAsset.UnitPointDistance(position.Value, trainInFrontPosition);

                        // when calculating stop position, include length of train
                        var unitPointDistanceToEndOfTrainInFront = unitPointsToTrainInFront - splineBlobAsset.GetSizeOfTrainInUnitPoint(ref settings);
                        var unitPointBrakingDistance = splineBlobAsset.DistanceToPointUnitDistance(settings.TrainBrakingDistance);
                        
                        if (unitPointDistanceToEndOfTrainInFront > unitPointBrakingDistance)
                            state.State = TrainMovementStates.Starting;
                        break;
                }
            }).ScheduleParallel();

        // update train positions based on current speed
        Entities.ForEach((ref TrainPosition position, in LineIndex lineIndex, in TrainMovement movement) =>
        {
            ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];
            
            position.Value += splineBlobAsset.DistanceToPointUnitDistance(movement.speed * deltaTime);
            position.Value = math.fmod(position.Value, splineBlobAsset.equalDistantPoints.Length);
        }).ScheduleParallel();
        
        m_SimulationECBSystem.AddJobHandleForProducer(Dependency);
    }
}

public static class UnitConvertExtensionMethods
{
    public static (float3, quaternion) PointUnitPosToWorldPos(this ref SplineBlobAsset splineBlob, float unitPointPos)
    {
        var floor = (int)math.floor(unitPointPos);
        var ceil = (floor+1) % splineBlob.equalDistantPoints.Length;

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
            if (unitPointPos < splineBlob.unitPointPlatformPositions[i]) return splineBlob.unitPointPlatformPositions[i] - unitPointPos;
        }

        // return distance to first platform (with overflow)
        return splineBlob.unitPointPlatformPositions[0] + (splineBlob.equalDistantPoints.Length - unitPointPos);
    }

    /// <summary>
    /// Returns distance between two points, handling overflow
    /// </summary>
    /// <param name="from">From position</param> 
    /// <param name="to">To position</param>
    /// <returns>Absolute distance in unit points</returns> 
    public static float UnitPointDistance(this ref SplineBlobAsset splineBlob, float from, float to)
    {
        if (from <= to)
            // without overflow, just return diff 
            return to - from;
        
        // overflow, so calculate remainder of line 
        return splineBlob.equalDistantPoints.Length - from + to;
    }
    
    public static Entity GetNextPlatformEntity(this ref SplineBlobAsset splineBlob, ref DynamicBuffer<EntityBufferElement> platformEntities, float unitPointPos)
    {
        unitPointPos %= splineBlob.equalDistantPoints.Length;
        for (var i = 0; i < splineBlob.unitPointPlatformPositions.Length; i++)
        {
            if (unitPointPos < splineBlob.unitPointPlatformPositions[i]) return platformEntities[i];
        }

        return platformEntities[0];
    }

    
    public static float DistanceToPointUnitDistance(this ref SplineBlobAsset splineBlob, float distance)
    {
        return distance / splineBlob.length * splineBlob.equalDistantPoints.Length;
    }
    
    public static float GetSizeOfTrainInUnitPoint(this ref SplineBlobAsset splineBlob, ref SettingsBlobAsset settings)
    {
        return splineBlob.DistanceToPointUnitDistance(splineBlob.CarriagesPerTrain * settings.CarriageSizeWithMargins);
    }
}
