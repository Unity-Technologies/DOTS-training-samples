using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothTimestepSystem))]
public class ClothProjectSystem : JobComponentSystem
{
    [BurstCompile]
    struct ProjectClothPositionsJob : IJobForEach_BBBBC<ClothProjectedPosition, ClothCurrentPosition, ClothPreviousPosition, ClothPinWeight, ClothTimestepData>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothCurrentPosition> currentPositions, 
            DynamicBuffer<ClothPreviousPosition> previousPositions, 
            DynamicBuffer<ClothPinWeight> pinWeights, 
            ref ClothTimestepData timestepData)
        {
            var fixedStepSq = timestepData.FixedTimestep * timestepData.FixedTimestep;
            
            var vertexCount = projectedPositions.Length;
            for (int vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
            {
                var currentPosition = currentPositions[vertexIndex].Value;
                
                var velocity = currentPosition - previousPositions[vertexIndex].Value;
                var gravity = new float3(0.0f, -9.8f, 0.0f) * fixedStepSq;
                velocity += gravity;

                var newProjected = currentPosition + velocity * pinWeights[vertexIndex].InvPinWeight;
                projectedPositions[vertexIndex] = new ClothProjectedPosition {Value = newProjected};
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ProjectClothPositionsJob();
        return job.Schedule(this, inputDependencies);
    }
}