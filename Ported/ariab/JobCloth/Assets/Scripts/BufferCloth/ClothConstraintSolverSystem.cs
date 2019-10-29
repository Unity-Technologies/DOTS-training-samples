using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothProjectSystem))]
public class ClothConstraintSolverSystem : JobComponentSystem
{
    [BurstCompile]
    struct DistanceConstraintSolver : IJobForEach_BBB<ClothProjectedPosition, ClothDistanceConstraint, ClothPinWeight>
    {
        public void Execute(DynamicBuffer<ClothProjectedPosition> projectedPositions, DynamicBuffer<ClothDistanceConstraint> distanceConstraints, DynamicBuffer<ClothPinWeight> pinWeights)
        {
            var iterationCount = 8;
            for (int iterationIndex = 0; iterationIndex < iterationCount; ++iterationIndex)
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
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new DistanceConstraintSolver();
        return job.Schedule(this, inputDependencies);
    }
}