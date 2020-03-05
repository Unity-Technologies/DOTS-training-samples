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

// See ClothSolverSystemGroup
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
        public BufferFromEntity<ClothPreviousPosition> previousPositionBuffer;
        
        public void Execute(Entity e, int index,
            [ReadOnly]DynamicBuffer<ClothHierarchicalParentIndexAndWeights> parentInfoBuffer, 
            [ReadOnly]ref ClothHierarchyParentEntity parentEntity)
        {
            var currentLevelPositions = projectedPositionBuffer[e];
            
            var projectedPositionArray = projectedPositionBuffer[parentEntity.Parent];
            var previousPositionArray = previousPositionBuffer[parentEntity.Parent];

            // Foreach projected position in the current level...
            for (int currentLevelPositionIndex = 0; currentLevelPositionIndex < currentLevelPositions.Length; ++currentLevelPositionIndex)
            {
                var parentIndicesAndWeights = parentInfoBuffer[currentLevelPositionIndex];

                // Calculate the weighted average position delta of all of the parent vertices
                var weightedDelta = float3.zero;
                for (int indexOfParentIndex = 0; indexOfParentIndex < parentIndicesAndWeights.ParentCount; ++indexOfParentIndex)
                {
                    var parentIndex = parentIndicesAndWeights.ParentIndex[indexOfParentIndex];

                    var parentCurrentPosition = projectedPositionArray[parentIndex].Value;
                    var parentPreviousPosition = previousPositionArray[parentIndex].Value;
                
                    var delta = parentCurrentPosition - parentPreviousPosition;
                    
                    var positionWeight = parentIndicesAndWeights.WeightValue[indexOfParentIndex];
                    weightedDelta += delta * positionWeight;
                }
                
                var currentPosition = currentLevelPositions[currentLevelPositionIndex].Value;
                currentLevelPositions[currentLevelPositionIndex] = new ClothProjectedPosition {Value = currentPosition + weightedDelta};
            }
        }
    }
    
    [BurstCompile]
    unsafe struct DistanceConstraintSolver : IJobForEach_BBBB<ClothProjectedPosition, ClothPreviousPosition, ClothDistanceConstraint, ClothPinWeight>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothPreviousPosition> previousPositions, 
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

    protected override void OnCreate()
    {
        m_LevelQuery = GetEntityQuery(
            ComponentType.ReadOnly<ClothHierarchyDepth>(),
            ComponentType.ReadOnly<ClothHierarchicalParentIndexAndWeights>(),
            ComponentType.ReadOnly<ClothHierarchyParentEntity>(),
            ComponentType.ReadOnly<ClothDistanceConstraint>(),
            ComponentType.ReadOnly<ClothPinWeight>(),
            ComponentType.ReadWrite<ClothProjectedPosition>(),
            ComponentType.ReadWrite<ClothPreviousPosition>()
            );
        
        m_LevelNoParentQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new []{
                    ComponentType.ReadOnly<ClothHierarchyDepth>(),
                    ComponentType.ReadOnly<ClothDistanceConstraint>(),
                    ComponentType.ReadOnly<ClothPinWeight>(),
                    ComponentType.ReadWrite<ClothProjectedPosition>(),
                    ComponentType.ReadWrite<ClothPreviousPosition>()
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
        var projectedPositionFromEntity = GetBufferFromEntity<ClothProjectedPosition>();
        var previousPositionFromEntity = GetBufferFromEntity<ClothPreviousPosition>(true);

        m_AllHierarchyDepths.Clear();
        EntityManager.GetAllUniqueSharedComponentData(m_AllHierarchyDepths);
        //m_AllHierarchyDepths.Sort();

        // First solve all levels with no parents
        var maxHierarchyLevel = m_AllHierarchyDepths.Count - 1;
        var waitHandle = inputDeps;
        for (int i = 0; i < 2; ++i)
        {
            waitHandle = new DistanceConstraintSolver().Schedule(m_LevelNoParentQuery, waitHandle);
        }

        // Then solve the rest hierarchically
        for (int levelIndex = maxHierarchyLevel - 1; levelIndex >= 0; levelIndex--)
        {
            m_LevelQuery.SetSharedComponentFilter(m_AllHierarchyDepths[levelIndex]);

            waitHandle = new PropagateOneHierarchyLevelJob
            {
                projectedPositionBuffer = projectedPositionFromEntity,
                previousPositionBuffer = previousPositionFromEntity
            }.Schedule(m_LevelQuery, waitHandle);

            for (int i = 0; i < (levelIndex+1); ++i)
            {
                waitHandle = new DistanceConstraintSolver().Schedule(m_LevelQuery, waitHandle);
            }
        }

        return waitHandle;
    }
}
