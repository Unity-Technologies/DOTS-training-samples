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

        var lineEntities = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadOnly<EntityBufferElement>()},
            None = new ComponentType[] {typeof(LineEntityHolder)}
        }).ToEntityArray(Allocator.TempJob);
        
        var lookup = GetBufferFromEntity<EntityBufferElement>(true);
        var ecb = m_SimulationECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities.WithReadOnly(lookup).WithDisposeOnCompletion(lineEntities)
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref TrainMovement movement, ref TrainState state, in LineIndex lineIndex) =>
        {
            ref var splineBlobAsset = ref splineData.Value.splineBlobAssets[lineIndex.Index];

            switch (state.State)
            {
                case TrainMovementStates.Starting:
                    movement.speed += settings.MaxSpeed*settings.Acceleration*deltaTime;
                    if (movement.speed >= settings.MaxSpeed)
                        state.State = TrainMovementStates.Running;
                    break;
                
                case TrainMovementStates.Stopping:
                    // following are in meters 
                    var unitPointDistance = splineBlobAsset.UnitPointDistanceToClosestPlatform(movement.position);
                    if (unitPointDistance > movement.distanceToStation) // Keep running if the platform was passed while transitioning to Stopped
                    {
                        state.State = TrainMovementStates.Running;
                        break;
                    }
                    movement.speed = (unitPointDistance / movement.distanceToStation) * settings.MaxSpeed;

                    if (movement.speed <= 0.0f || unitPointDistance < .1f)
                    {
                        movement.speed = 0.0f;
                        state.State = TrainMovementStates.Waiting;
                        var lineEntity = lineEntities[lineIndex.Index];
                        var platformEntities = lookup[lineEntity];
                        var platformEntity = splineBlobAsset.GetNextPlatformEntity(ref platformEntities, unitPointDistance);
                        ecb.SetComponent(entityInQueryIndex, platformEntity, new Occupancy{Train = e, TimeLeft = settings.TimeAtStation});
                    }
                    break;

                case TrainMovementStates.Running:
                    // are we approaching platform?
                    var unitPointDistToClosestPlatform = splineBlobAsset.UnitPointDistanceToClosestPlatform(movement.position);
                    if (unitPointDistToClosestPlatform < splineBlobAsset.DistanceToPointUnitDistance(settings.BreakingDistance))
                    {
                        movement.distanceToStation = unitPointDistToClosestPlatform;
                        state.State = TrainMovementStates.Stopping;
                    }

                    break;
            }
            
            movement.position += splineBlobAsset.DistanceToPointUnitDistance(movement.speed * deltaTime);
            movement.position = math.fmod(movement.position, splineBlobAsset.equalDistantPoints.Length);
            (translation.Value, _) = splineBlobAsset.PointUnitPosToWorldPos(movement.position);
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
