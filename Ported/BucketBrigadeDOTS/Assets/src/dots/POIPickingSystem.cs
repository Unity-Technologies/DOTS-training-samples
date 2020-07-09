using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FireSpreadSystem))]
public class POIPickingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        GetEntityQuery(typeof(FireCellFlag));
        GetEntityQuery(typeof(FireGridMipLevelData));
    }

    static void PickClosestPoint(NativeArray<FireCellFlag> mipChainBuffer, NativeArray<FireGridMipLevelData> mipChainInfoBuffer,
                                    float2 referencePosition, int currentMip, int2 mipOffset, float2 parentCellPosition,
                                    ref float closestDistance, ref float2 closestPosition, ref int numIterations)
    {
        numIterations++;
        // Get the data of the current mip
        int2 mipLevelDimensions = (int2)mipChainInfoBuffer[currentMip].dimensions;
        int mipLevelOffset = (int)mipChainInfoBuffer[currentMip].offset;

        // Compute the center pos of the cell
        float2 localSpaceReferencePosition = referencePosition - parentCellPosition;

        int3 loopRangeX = new int3(mipOffset.x, mipOffset.x + 1, 1);
        if (localSpaceReferencePosition.x > 0 )
        {
            loopRangeX.x = mipOffset.x + 1;
            loopRangeX.y = mipOffset.x;
            loopRangeX.z = -1;
        }
        int3 loopRangeY = new int3(mipOffset.y, mipOffset.y + 1, 1);
        if (localSpaceReferencePosition.y > 0)
        {
            loopRangeY.x = mipOffset.y + 1;
            loopRangeY.y = mipOffset.y;
            loopRangeY.z = -1;
        }
        // Loop through the cells
        for (int v = loopRangeY.x; v != (loopRangeY.y + loopRangeY.z); v = v + loopRangeY.z)
        {
            for (int u = loopRangeX.x; u != (loopRangeX.y + loopRangeX.z); u = u + loopRangeX.z)
            {
                float2 cellCenterPosition = mipChainInfoBuffer[currentMip].minCellPosition + mipChainInfoBuffer[currentMip].cellSize * new float2(u, v);

                // If this cell is flagged as on fire
                if (mipChainBuffer[mipLevelOffset + u + v * mipLevelDimensions.x].OnFire)
                {
                    if (currentMip == 0)
                    {
                        // Convert the position of the cell to world and check with the closest distance
                        float2 distVec = referencePosition - cellCenterPosition;
                        float newDistanceCandidate = math.dot(distVec, distVec);
                        if (newDistanceCandidate < closestDistance)
                        {
                            closestPosition = cellCenterPosition;
                            closestDistance = newDistanceCandidate;
                        }
                    }
                    else
                    {
                        // TODO, evaluate the minimal distance of the cell and check that with the current distance
                        float2 projectedPoint = math.max(cellCenterPosition - mipChainInfoBuffer[currentMip].cellSize * 0.5f, 
                                                math.min(cellCenterPosition + mipChainInfoBuffer[currentMip].cellSize * 0.5f, referencePosition));
                        float2 bestVec = (projectedPoint - referencePosition);
                        float bestPotentialDistance = math.dot(bestVec, bestVec);

                        if (bestPotentialDistance < closestDistance)
                        {
                            // Move to the next mip Level
                            int2 newMipOffset = new int2(u, v) * 2;
                            PickClosestPoint(mipChainBuffer, mipChainInfoBuffer,
                                referencePosition, currentMip - 1, newMipOffset, cellCenterPosition, 
                                ref closestDistance, ref closestPosition, ref numIterations);
                        }
                    }
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        // Grab all the data we need
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGridSetting = GetComponent<FireGridSettings>(fireGridEntity);
        var fireGridBounds = GetComponent<Bounds>(fireGridEntity);
        var mipChainBuffer = EntityManager.GetBuffer<FireCellFlag>(fireGridEntity).AsNativeArray();
        var mipChainInfoBuffer = EntityManager.GetBuffer<FireGridMipLevelData>(fireGridEntity).AsNativeArray();

        // Compute the data require to evaluate the position of each cell of mip 0
        float2 boundsScale = fireGridBounds.BoundsExtent / (float2) fireGridSetting.FireGridResolution;
        float2 minGridPosition = fireGridBounds.BoundsCenter - fireGridBounds.BoundsExtent * 0.5f + boundsScale * 0.5f;

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities
        .WithReadOnly(mipChainBuffer)
        .WithReadOnly(mipChainInfoBuffer)
        .ForEach((Entity targetEntity, in PointOfInterestRequest fireGridSpawner) =>
        {
            // Initialize the distance
            float currentMinimalDistance = float.MaxValue;
            float2 currentTargetPoint = 0.0f;
            int numIterations = 0;
            // Do the decent
            PickClosestPoint(mipChainBuffer, mipChainInfoBuffer, 
                fireGridSpawner.POIReferencePosition, mipChainInfoBuffer.Length - 2, 0, fireGridBounds.BoundsCenter,
                ref currentMinimalDistance, ref currentTargetPoint, ref numIterations);

            // Remove the request component
            ecb.RemoveComponent<PointOfInterestRequest>(targetEntity);

            if (currentMinimalDistance < float.MaxValue)
            {
                // Add the result component
                PointOfInterestRequest poiEval;
                poiEval.POIReferencePosition = currentTargetPoint;
                ecb.AddComponent<PointOfInterestRequest>(targetEntity);
                ecb.SetComponent<PointOfInterestRequest>(targetEntity, poiEval);
            }
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
