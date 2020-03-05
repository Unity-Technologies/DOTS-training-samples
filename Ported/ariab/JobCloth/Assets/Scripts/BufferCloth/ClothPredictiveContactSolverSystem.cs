using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

// See ClothSolverSystemGroup
[DisableAutoCreation]
public class ClothPredictiveContactSolverSystem : JobComponentSystem
{
    [BurstCompile]
    struct ClothPredictiveContactSolver : IJobForEach_BB<ClothProjectedPosition, ClothCollisionContact>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothCollisionContact> contacts)
        {
            for (int i = 0; i < contacts.Length; ++i)
            {
                var currentContact = contacts[i];
                var projectedPosition = projectedPositions[currentContact.VertexIndex].Value;

                var normal = currentContact.ContactPlane.xyz;
                var dist = math.dot(projectedPosition, normal) - currentContact.ContactPlane.w;
                dist = math.min(dist, 0.0f);
                
                var delta = dist * normal;
                projectedPositions[currentContact.VertexIndex] = new ClothProjectedPosition{Value = projectedPosition - delta};
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ClothPredictiveContactSolver();
        return job.Schedule(this, inputDependencies);
    }
}