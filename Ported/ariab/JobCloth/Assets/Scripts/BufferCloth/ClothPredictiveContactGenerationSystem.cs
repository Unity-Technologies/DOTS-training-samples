using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

// See ClothSolverSystemGroup
[DisableAutoCreation]
public class ClothPredictiveContactGenerationSystem : JobComponentSystem
{
    [BurstCompile]
    struct ClothPredictiveContactGenerationSystemJob : IJobForEach_BBBBBB<ClothProjectedPosition, ClothCurrentPosition, ClothSphereCollider, ClothCapsuleCollider, ClothPlaneCollider, ClothCollisionContact>
    {
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothCurrentPosition> currentPositions, 
            DynamicBuffer<ClothSphereCollider> sphereColliders, 
            DynamicBuffer<ClothCapsuleCollider> capsuleColliders, 
            DynamicBuffer<ClothPlaneCollider> planeColliders, 
            DynamicBuffer<ClothCollisionContact> outGeneratedContacts)
        {
            // Clear the contacts buffer each time we run the contact generator
            outGeneratedContacts.Clear();

            // Many more vertices than spheres, vertices are outer loop
            for (int vertexIndex = 0; vertexIndex < projectedPositions.Length; ++vertexIndex)
            {
                var projectedPosition = projectedPositions[vertexIndex].Value;

                // Sphere Colliders
                for (int colliderIndex = 0; colliderIndex < sphereColliders.Length; ++colliderIndex)
                {
                    var sphere = sphereColliders[colliderIndex];

                    var distToSphereCenter = math.distance(projectedPosition, sphere.LocalCenter);
                    if (distToSphereCenter < sphere.Radius)
                    {
                        var currentPosition = currentPositions[vertexIndex].Value;
                        var contactPlaneNormal = math.normalize(currentPosition - sphere.LocalCenter);
                        var contactPlaneOffset = math.dot(sphere.LocalCenter, contactPlaneNormal) + sphere.Radius;

                        outGeneratedContacts.Add(new ClothCollisionContact
                        {
                            ContactPlane = new float4{ xyz = contactPlaneNormal, w = contactPlaneOffset},
                            VertexIndex = vertexIndex
                        });
                    }
                }

                // Capsule Colliders
                // todo

                // Plane Colliders
                for (int colliderIndex = 0; colliderIndex < planeColliders.Length; ++colliderIndex)
                {
                    var plane = planeColliders[colliderIndex];

                    var planeProjection = math.dot(projectedPosition, plane.LocalNormal);
                    if (planeProjection < plane.LocalOffset)
                    {
                        outGeneratedContacts.Add(new ClothCollisionContact
                        {
                            ContactPlane = new float4{xyz = plane.LocalNormal, w = plane.LocalOffset},
                            VertexIndex = vertexIndex
                        });
                    }
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new ClothPredictiveContactGenerationSystemJob();
        return job.Schedule(this, inputDependencies);
    }
} 