using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
public class ClothHierarchicalSolverSystem : JobComponentSystem
{
    private EntityQuery m_LevelNoParentQuery;
    private EntityQuery m_LevelQuery;
    private List<ClothHierarchyDepth> m_AllHierarchyDepths;
    
    [BurstCompile]
    unsafe struct PropagateOneHierarchyLevelJob : IJobForEachWithEntity_EBC<ClothHierarchicalParentIndexAndWeights, ClothHierarchyParentEntity>
    {
        [ReadOnly]
        [NativeDisableContainerSafetyRestriction]
        public BufferFromEntity<ClothProjectedPosition> projectedPositionBuffer;
        
        [ReadOnly]
        [NativeDisableContainerSafetyRestriction]
        public BufferFromEntity<ClothCurrentPosition> currentPositionBuffer;
        
        public void Execute(Entity e, int index,
            [ReadOnly]DynamicBuffer<ClothHierarchicalParentIndexAndWeights> parentInfoBuffer, 
            [ReadOnly]ref ClothHierarchyParentEntity parentEntity)
        {
            var currentLevelPositions = projectedPositionBuffer[e];
            var parentPositions = projectedPositionBuffer[parentEntity.Parent];
            var parentLastPositions = currentPositionBuffer[parentEntity.Parent];

            // Foreach projected position in the current level...
            for (int currentLevelPositionIndex = 0; currentLevelPositionIndex < currentLevelPositions.Length; ++currentLevelPositionIndex)
            {
                var parentIndicesAndWeights = parentInfoBuffer[currentLevelPositionIndex];

                // Calculate the weighted average position delta of all of the parent vertices
                var weightedDelta = float3.zero;
                for (int indexOfParentIndex = 0; indexOfParentIndex < parentIndicesAndWeights.ParentCount; ++indexOfParentIndex)
                {
                    var parentIndex = parentIndicesAndWeights.ParentIndex[indexOfParentIndex];

                    var parentLastPosition = parentLastPositions[parentIndex].Value;
                    var parentPosition = parentPositions[parentIndex].Value;
                
                    var delta = parentPosition - parentLastPosition;
                    
                    var positionWeight = parentIndicesAndWeights.WeightValue[indexOfParentIndex];
                    weightedDelta += delta * positionWeight;
                }
                var currentPosition = currentLevelPositions[currentLevelPositionIndex].Value;
                
                currentLevelPositions[currentLevelPositionIndex] = new ClothProjectedPosition {Value = currentPosition + weightedDelta};
            }
        }
    }
    
    [BurstCompile]
    struct DistanceConstraintSolver : IJobForEach_BBB<ClothProjectedPosition, ClothDistanceConstraint, ClothPinWeight>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            [ReadOnly]DynamicBuffer<ClothDistanceConstraint> distanceConstraints, 
            [ReadOnly]DynamicBuffer<ClothPinWeight> pinWeights)
        {
            for (int constraintIndex = 0; constraintIndex < distanceConstraints.Length; ++constraintIndex)
            {
                var indexA = distanceConstraints[constraintIndex].VertexA;
                var indexB = distanceConstraints[constraintIndex].VertexB;
                var restLengthSqr = distanceConstraints[constraintIndex].RestLengthSqr;

                var vertexA = projectedPositions[indexA].Value;
                var vertexB = projectedPositions[indexB].Value;

                var delta = vertexB - vertexA;
                delta *= (restLengthSqr / (math.dot(delta, delta) + restLengthSqr) - 0.5f);

                vertexA -= delta * pinWeights[indexA].InvPinWeight;
                vertexB += delta * pinWeights[indexB].InvPinWeight;

                projectedPositions[indexA] = new ClothProjectedPosition {Value = vertexA};
                projectedPositions[indexB] = new ClothProjectedPosition {Value = vertexB};
            }
        }
    }

    protected override void OnCreateManager()
    {
        m_LevelQuery = GetEntityQuery(
            ComponentType.ReadOnly<ClothHierarchyDepth>(),
            ComponentType.ReadOnly<ClothHierarchicalParentIndexAndWeights>(),
            ComponentType.ReadOnly<ClothHierarchyParentEntity>(),
            ComponentType.ReadOnly<ClothDistanceConstraint>(),
            ComponentType.ReadOnly<ClothPinWeight>(),
            ComponentType.ReadWrite<ClothProjectedPosition>()
            );
        
        m_LevelNoParentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new []{
                    ComponentType.ReadOnly<ClothHierarchyDepth>(),
                    ComponentType.ReadOnly<ClothDistanceConstraint>(),
                    ComponentType.ReadOnly<ClothPinWeight>(),
                    ComponentType.ReadWrite<ClothProjectedPosition>()
                },
                None = new []
                {
                    ComponentType.ReadOnly<ClothHierarchyParentEntity>(),
                    ComponentType.ReadOnly<ClothHierarchicalParentIndexAndWeights>()
                }
            }
        );
        
        m_AllHierarchyDepths = new List<ClothHierarchyDepth>(10);
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var positionBufferFromEntity = GetBufferFromEntity<ClothProjectedPosition>();
        var oldPositionBufferFromEntity = GetBufferFromEntity<ClothCurrentPosition>(true);

        m_AllHierarchyDepths.Clear();
        EntityManager.GetAllUniqueSharedComponentData(m_AllHierarchyDepths);
        //m_AllHierarchyDepths.Sort();

        // First solve all levels with no parents
        var solveFirstLevelHandle = new DistanceConstraintSolver().Schedule(m_LevelNoParentQuery, inputDeps);

        // Then solve the rest hierarchically
        var maxHierarchyLevel = m_AllHierarchyDepths.Count - 1;
        var waitOnPreviousLevelHandle = solveFirstLevelHandle;
        for (int levelIndex = maxHierarchyLevel - 1; levelIndex >= 0; levelIndex--)
        {
            m_LevelQuery.SetFilter(m_AllHierarchyDepths[levelIndex]);

            var propagateResultsDownHierarchyHandle = new PropagateOneHierarchyLevelJob
            {
                projectedPositionBuffer = positionBufferFromEntity,
                currentPositionBuffer = oldPositionBufferFromEntity
            }.Schedule(m_LevelQuery, waitOnPreviousLevelHandle);

            waitOnPreviousLevelHandle = new DistanceConstraintSolver().Schedule(m_LevelQuery, propagateResultsDownHierarchyHandle);
        }

        return waitOnPreviousLevelHandle;
    }
}
