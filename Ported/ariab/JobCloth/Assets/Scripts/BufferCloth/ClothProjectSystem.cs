using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothTimestepSystem))]
public class ClothProjectSystem : JobComponentSystem
{
    [BurstCompile]
    struct ProjectClothPositionsJob : IJobForEach_BBBBCC<ClothProjectedPosition, ClothCurrentPosition, ClothPreviousPosition, ClothPinWeight, ClothTimestepData, ClothWorldToLocal>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothCurrentPosition> currentPositions, 
            DynamicBuffer<ClothPreviousPosition> previousPositions, 
            DynamicBuffer<ClothPinWeight> pinWeights, 
            ref ClothTimestepData timestepData,
            ref ClothWorldToLocal worldToLocal)
        {
            var fixedStepSq = timestepData.FixedTimestep * timestepData.FixedTimestep;
            
            var vertexCount = projectedPositions.Length;
            for (int vertexIndex = 0; vertexIndex < vertexCount; ++vertexIndex)
            {
                var currentPosition = currentPositions[vertexIndex].Value;
                
                var velocity = currentPosition - previousPositions[vertexIndex].Value;
                var gravity = math.mul(worldToLocal.Value, new float4(0.0f, -9.8f, 0.0f, 0.0f)).xyz * fixedStepSq;
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