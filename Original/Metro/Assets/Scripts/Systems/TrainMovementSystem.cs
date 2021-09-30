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
        var settings = GetSingleton<Settings>();
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

                switch (state.State)
                {
                    case TrainMovementStates.Starting:
                        movement.speed += settings.MaxSpeed * settings.Acceleration * deltaTime;
                        if (movement.speed >= settings.MaxSpeed)
                            state.State = TrainMovementStates.Running;
                        break;

                    case TrainMovementStates.Stopping:
                        var unitPointDistance = splineBlobAsset.UnitPointDistanceToClosestPlatform(position.Value);
                        if (unitPointDistance > movement.distanceToStop) // Keep running if the platform was passed while transitioning to Stopped
                        {
                            state.State = TrainMovementStates.Running;
                            break;
                        }

                        movement.speed = (unitPointDistance / movement.distanceToStop) * settings.MaxSpeed;

                        if (movement.speed <= 0.0f || unitPointDistance < .1f)
                        {
                            movement.speed = 0.0f;
                            state.State = TrainMovementStates.Waiting;
                            var lineEntity = lineEntities[lineIndex.Index];
                            var platformEntities = lookup[lineEntity];
                            var platformEntity = splineBlobAsset.GetNextPlatformEntity(ref platformEntities, unitPointDistance);
                            ecb.SetComponent(entityInQueryIndex, platformEntity, new Occupancy {Train = e, TimeLeft = settings.TimeAtStation});
                        }

                        break;

                    case TrainMovementStates.Running:
                        // are we approaching train in front?
                        float unitPointsToTrainInFront;
                        var trainInFrontPosition = GetComponent<TrainPosition>(trainInFront.Train).Value;

                        if (position.Value < trainInFrontPosition)
                        {
                            unitPointsToTrainInFront = trainInFrontPosition - position.Value;
                        }
                        else
                        {
                            // train in front has "overflown" train line array
                            unitPointsToTrainInFront = splineBlobAsset.equalDistantPoints.Length - position.Value + trainInFrontPosition;
                        }

                        var unitPointBreakingDistance = splineBlobAsset.DistanceToPointUnitDistance(settings.TrainBreakingDistance);
                        if (unitPointsToTrainInFront < unitPointBreakingDistance)
                        {
                            movement.distanceToStop = unitPointsToTrainInFront;
                            state.State = TrainMovementStates.Stopping;
                            break;
                        }

                        // are we approaching platform?
                        var unitPointDistToClosestPlatform = splineBlobAsset.UnitPointDistanceToClosestPlatform(position.Value);
                        unitPointBreakingDistance = splineBlobAsset.DistanceToPointUnitDistance(settings.PlatformBreakingDistance);
                        if (unitPointDistToClosestPlatform < unitPointBreakingDistance)
                        {
                            movement.distanceToStop = unitPointDistToClosestPlatform;
                            state.State = TrainMovementStates.Stopping;
                        }

                        break;
                }
            }).ScheduleParallel();

        // update train positions based on current speed
        Entities.ForEach((ref TrainPosition position, ref Translation translation, in LineIndex lineIndex, in TrainMovement movement) =>
        {
            ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];
            
            position.Value += splineBlobAsset.DistanceToPointUnitDistance(movement.speed * deltaTime);
            position.Value = math.fmod(position.Value, splineBlobAsset.equalDistantPoints.Length);
            (translation.Value, _) = splineBlobAsset.PointUnitPosToWorldPos(position.Value);
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

        return splineBlob.unitPointPlatformPositions[0] + (splineBlob.equalDistantPoints.Length - unitPointPos);
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
}
