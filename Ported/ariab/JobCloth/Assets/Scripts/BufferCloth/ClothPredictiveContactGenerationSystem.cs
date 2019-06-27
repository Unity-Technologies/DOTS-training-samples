// There are two methods to apply internal cloth-cloth intersection
// Bridson, Fedkiw, Anderson ( Robust Treatment of Collisions, Contact and Friction for Cloth Animation ) 
// recommend an impulse method which seems more stable and simpler than the constraint based method in
// Chris Lewin (Cloth Self Collision with Predictive Contacts, gdc18 )


//#define APPLY_CONSTRAINTS



using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using static Unity.Physics.Math;
using Unity.Physics.Extensions;
using UnityEngine;

using PointDistanceInput = Unity.Physics.PointDistanceInput;
using PointHit = Unity.Physics.DistanceHit;
using RigidBody = Unity.Physics.RigidBody;


struct InternalImpulse
{
    public float3 Normal;
    public float Value;
    public int Count;
    public int VertexIndex;
};

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ClothProjectSystem))]
public class ClothPredictiveContactGenerationSystem : JobComponentSystem
{
    [ReadOnly]
    private BuildPhysicsWorld CreatePhysicsWorldSystem;

    [BurstCompile]
    struct ClothPredictiveContactGenerationSystemJob : IJobForEach_BBBC<ClothProjectedPosition, ClothCurrentPosition, ClothCollisionContact, ClothWorldToLocal>
    {
        [ReadOnly]
        public CollisionWorld world;
        [ReadOnly]
        public PhysicsWorld physicsWorld;

        public float deltaTime;
        public float contactRadius;

        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions, 
            DynamicBuffer<ClothCurrentPosition> currentPositions, 
            DynamicBuffer<ClothCollisionContact> outGeneratedContacts,
            ref ClothWorldToLocal worldToLocal)
        {
            // Clear the contacts buffer each time we run the contact generator
            outGeneratedContacts.Clear();
            float4x4 localToWorld = math.inverse(worldToLocal.Value);

            // Many more vertices than spheres, vertices are outer loop
            for (int vertexIndex = 0; vertexIndex < projectedPositions.Length; ++vertexIndex)
            {
                // Put the movement into world space, so we can collide with the physics world objects
                var currentPosition = math.transform(localToWorld,currentPositions[vertexIndex].Value);
                var projectedPosition = math.transform(localToWorld,projectedPositions[vertexIndex].Value);

                {
                    // check here for penetration constraints
                    PointDistanceInput distanceInput = new PointDistanceInput
                    {
                        Position = currentPosition,
                        MaxDistance = contactRadius + math.length(projectedPosition - currentPosition),
                        Filter = CollisionFilter.Default,
                    };
              
                    // Find all the closest objects within range
                    NativeList<PointHit> penetrationPointList = new NativeList<PointHit>(Allocator.Temp);
                 
                    if ( world.CalculateDistance(distanceInput, ref penetrationPointList))
                    {
                        // For every collideable object in range we need to check for a potential collision
                        for (int i=0; i< penetrationPointList.Length; ++i )
                        {
                            PointHit penetrationPoint = penetrationPointList[i];
                        
                            // If we're inside the object?, or within the collision radius
                            if (penetrationPoint.Distance< contactRadius)
                            {
                                var rigidBodyIndex = penetrationPoint.RigidBodyIndex;

                                // Get the combined velocity of the closest point and the particle
                                float3 lv = PhysicsWorldExtensions.GetLinearVelocity(physicsWorld, rigidBodyIndex, penetrationPoint.Position);
                                lv *= deltaTime;

                                float3 surfaceNormal = penetrationPoint.SurfaceNormal;

                                // Get a world space rotation quaternion for the velociy
                                Quaternion dq = Integrator.IntegrateAngularVelocity(physicsWorld.GetAngularVelocity(rigidBodyIndex), deltaTime);
                                dq = math.normalize(dq);

                                // rotate the surface normal through this quat, to give target plane normal
                                surfaceNormal = math.rotate(dq, surfaceNormal);

                                float3 contactPoint = (penetrationPoint.Position + lv);

                                // put the contact information back into the local space of the cloth
                                surfaceNormal = math.rotate(worldToLocal.Value, surfaceNormal);
                                contactPoint = math.transform(worldToLocal.Value, contactPoint);

                                var planeDistance = math.dot(surfaceNormal, contactPoint) + contactRadius;
                           
                                outGeneratedContacts.Add(new ClothCollisionContact
                                {
                                    ContactPlane = new float4 { xyz = surfaceNormal, w = planeDistance },
                                    VertexIndex = vertexIndex,
                                });
                            }
                            else
                            {
                         
                                var rigidBodyIndex = penetrationPoint.RigidBodyIndex;

                                // Get the combined velocity of the closest point and the particle
                                float3 lv = PhysicsWorldExtensions.GetLinearVelocity(physicsWorld, rigidBodyIndex, penetrationPoint.Position);
                                lv *= deltaTime;
                                float3 clv = -lv + (projectedPosition - currentPosition);

                                // Re-test against the  rigid body collidables
                                RigidBody body = physicsWorld.Bodies[rigidBodyIndex];

                                float3 diff = (penetrationPoint.Position - currentPosition);

                                // normalize the movement direction for the particle
                                float len = math.length(diff);
                                diff *= (1.0f / len);

                                // Get the movement in the direction of the contact point
                                float dp = math.dot(diff, clv);

                                // If we're likely to travel far enough in this direct
                                if ( (len - dp ) < contactRadius )
                                {
                                    float3 surfaceNormal = penetrationPoint.SurfaceNormal;

                                    // Get a world space rotation quaternion for the velociy
                                    Quaternion dq = Integrator.IntegrateAngularVelocity(physicsWorld.GetAngularVelocity(rigidBodyIndex),deltaTime);
                                    dq = math.normalize(dq);

                                    // rotate the surface normal through this quat, to give target plane normal
                                    surfaceNormal = math.rotate(dq, surfaceNormal);

                                    // As this is the end of the frame, rotate the constraint into the expected angle
                                    surfaceNormal = math.rotate(worldToLocal.Value, surfaceNormal);

                                    float3 contactPoint = penetrationPoint.Position + lv;

                                    contactPoint = math.transform(worldToLocal.Value, contactPoint);

                                   
                                    //var planeDistance = math.dot(penetrationPoint.SurfaceNormal, penetrationPoint.Position) + penetrationPoint.Distance;
                                    var planeDistance = math.dot(surfaceNormal, contactPoint) + contactRadius;

                                    outGeneratedContacts.Add(new ClothCollisionContact
                                    {
                                        ContactPlane = new float4 { xyz = surfaceNormal, w = planeDistance },
                                        VertexIndex = vertexIndex
                                    });
                                }

                            }
                        
                        }
                        
                    }
                }
            }
        }
    }

    [BurstCompile]
    struct ClothIntersectionContactJob : IJobForEach_BBBBC<ClothProjectedPosition, ClothCurrentPosition, ClothTriangle, ClothCollisionContact, ClothConstraintSetup>
    {
        [ReadOnly]
        public CollisionWorld world;
        [ReadOnly]
        public PhysicsWorld physicsWorld;

        public float deltaTime;
        public float contactRadius;

        // Find the cloest point on a triangle and find its barycentric coordinates
        // As described in David Eberly Geometric Tools for Computer Graphics ( GeometricTools.com )
        private float3 ClosestPointOnTriangle( float3 vp0, float3 vp1, float3 vp2, float3 sourcePosition, out float os, out float ot )
        {
            float3 edge0 = vp1 - vp0;
            float3 edge1 = vp2 - vp0;
            float3 v0    = vp0 - sourcePosition;

            float a = math.dot(edge0,edge0);
            float b = math.dot(edge0,edge1);
            float c = math.dot(edge1,edge1);
            float d = math.dot(edge0,v0);
            float e = math.dot(edge1,v0);

            float det = a * c - b * b;
            float s = b * e - c * d;
            float t = b * d - a * e;

            if (s + t<det )
            {
                if (s< 0.0f )
                {
                    if (t< 0.0f )
                    {
                        if (d< 0.0f )
                        {
                           s = math.clamp( -d/a, 0.0f, 1.0f );
                           t = 0.0f;
                        }
                        else
                        {
                            s = 0.0f;
                            t = math.clamp( -e/c, 0.0f, 1.0f );
                        }
                    }
                    else
                    {
                        s = 0.0f;
                        t = math.clamp( -e/c, 0.0f, 1.0f );
                    }
                }
                else if (t< 0.0f )
                {
                    s = math.clamp( -d/a, 0.0f, 1.0f );
                    t = 0.0f;
                }
                else
                {
                    float invDet = 1.0f / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s< 0.0f )
                {
                    float tmp0 = b + d;
                    float tmp1 = c + e;
                    if (tmp1 > tmp0 )
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2 * b + c;
                        s = math.clamp(numer/denom, 0.0f, 1.0f );
                        t = 1-s;
                    }
                    else
                    {
                        t = math.clamp( -e/c, 0.0f, 1.0f );
                        s = 0.0f;
                    }
                }
                else if (t< 0.0f )
                {
                    if (a+d > b+e )
                    {
                        float numer = c + e - b - d;
                        float denom = a - 2 * b + c;
                        s = math.clamp(numer/denom, 0.0f, 1.0f );
                        t = 1-s;
                    }
                    else
                    {
                        s = math.clamp( -e/c, 0.0f, 1.0f );
                        t = 0.0f;
                    }
                }
                else
                {
                    float numer = c + e - b - d;
                    float denom = a - 2 * b + c;
                    s = math.clamp(numer/denom, 0.0f, 1.0f );
                    t = 1.0f - s;
                }
            }

            ot = t;
            os = s;

            return vp0 + s* edge0 + t* edge1;
        }


        // The original paper discusses using the triangle edge planes to form constraints for the edge pieces
        // but for now, we're just pushing the triangle backwards in its own plane
        private void AddTriangleConstraint(ref DynamicBuffer<ClothCollisionContact> outGeneratedContacts, float3 normal, float planeDistance, int vertexID, float3 vert, float weight, float3 edge )
        {
             // Add constraint for p1
            outGeneratedContacts.Add(new ClothCollisionContact
            {
                ContactPlane = new float4 { xyz = normal, w = planeDistance },
                VertexIndex = vertexID
            });
        }

        // Cloth intersection. To prevent self intersection, where cloth can return to a natural shape
        // ie should not be used on cloth falling onto a surface where constraints become badly formed, but can help with pinned cloth

        // All of the calculations for cloth intersection take place in the local space of the cloth
        public void Execute(
            DynamicBuffer<ClothProjectedPosition> projectedPositions,
            DynamicBuffer<ClothCurrentPosition> currentPositions,
            DynamicBuffer<ClothTriangle> triangles,
            DynamicBuffer<ClothCollisionContact> outGeneratedContacts,
            ref ClothConstraintSetup constraintSetup)
        {

            // If this entity does not require self intersection tests, then don't do it
            if (!constraintSetup.SelfIntersection)
            {
                return;
            }

            // Generate bounding boxes for each triangle in the mesh
            NativeArray<Aabb> boundingBoxes = new NativeArray<Aabb>(triangles.Length, Allocator.Temp);

#if !APPLY_CONSTRAINTS
            NativeList<InternalImpulse> impulses = new NativeList<InternalImpulse>(projectedPositions.Length,Allocator.Temp);
            NativeArray<int> impulseCounts = new NativeArray<int>(projectedPositions.Length, Allocator.Temp);
#endif

            float feps = 0.0025f + contactRadius;
            float3 eps = new float3(feps,feps,feps);

            float3 bbAverage = new float3();
            Aabb meshBounds = new Aabb { Min = new float3(float.MaxValue), Max = new float3(float.MinValue) };

            for ( int i =0; i<triangles.Length; ++i )
            {
                // Calculate a bounding box over the extruded movement of the triangle
                var v0 = triangles[i].v0;
                var v1 = triangles[i].v1;
                var v2 = triangles[i].v2;

                // test swept movement against initial state
                var vc0 = currentPositions[v0].Value;
                var vc1 = currentPositions[v1].Value;
                var vc2 = currentPositions[v2].Value;

                var vp0 = projectedPositions[v0].Value;
                var vp1 = projectedPositions[v1].Value;
                var vp2 = projectedPositions[v2].Value;

                var vMin = math.min(math.min(math.min(vc0, vc1), vc2), math.min(math.min(vp0, vp1), vp2));
                var vMax = math.max(math.max(math.max(vc0, vc1), vc2), math.max(math.max(vp0, vp1), vp2));

                boundingBoxes[i] = new Aabb { Min = vMin - eps, Max = vMax + eps, };

                bbAverage += (vMax-vMin);
                meshBounds.Include(boundingBoxes[i]);

            }

            // TODO - does it make sense to pre-group the triangle based on the mesh into regions ?
            // And then just update the grid box bounds in the first pass

            // Would maybe have to sort and those bounds again, if we wanted to go hierarchical
            // other issue would occur in twisted scenes

            // calculate a grid size
            meshBounds.Expand(feps);
            float3 meshDims = (meshBounds.Max - meshBounds.Min);
            bbAverage = meshDims / bbAverage;
            int3 gridSize = (int3)math.ceil(bbAverage)*6;

            float3 gridBoxDims = (meshDims) / gridSize;
            float3 ooGridBoxDims = new float3(1.0f, 1.0f, 1.0f) / gridBoxDims;

            NativeArray<int> gridCounters = new NativeArray<int>(gridSize.x * gridSize.y * gridSize.z, Allocator.Temp);
            NativeArray<int> gridBase = new NativeArray<int>(gridSize.x * gridSize.y * gridSize.z, Allocator.Temp);
            NativeArray<int> triangleIndex = new NativeArray<int>(triangles.Length, Allocator.Temp);
            NativeArray<int> gridIndices = new NativeArray<int>(triangles.Length, Allocator.Temp);

            // Classify the triangles into the grid
            for (int i = 0; i < triangles.Length; ++i)
            {
                int3 pos = (int3)math.floor((boundingBoxes[i].Min - meshBounds.Min) * ooGridBoxDims);
                int index = pos.x + pos.y * gridSize.x + pos.z * gridSize.x * gridSize.y;

                triangleIndex[i] = index;
                gridCounters[index] += 1;
            }

            int baseValue = 0;
            for (int i = 0; i < gridBase.Length; ++i)
            {
                gridBase[i] = baseValue;
                baseValue += gridCounters[i];
            }

            // Push the triangle bounds into the bins
            for (int i = 0; i < triangles.Length; ++i)
            {
                int index = triangleIndex[i];
                gridIndices[gridBase[index]] = i;
                gridBase[index] += 1;
            }

            // Generate a bounding box from each bin
            baseValue = 0;
            for (int i = 0; i < gridBase.Length; ++i)
            {
                gridBase[i] = baseValue;
                baseValue += gridCounters[i];
            }


            NativeArray<Aabb> gridBounds = new NativeArray<Aabb>(gridSize.x * gridSize.y * gridSize.z, Allocator.Temp);

            for ( int i = 0; i< gridBounds.Length; ++i )
            {
                Aabb blockBounds = new Aabb { Min = new float3(float.MaxValue), Max = new float3(float.MinValue) };

                for ( int j=0; j<gridCounters[i]; ++j)
                {
                    int index = gridIndices[gridBase[i] + j];

                    blockBounds.Include(boundingBoxes[index]);
                }

                gridBounds[i] = blockBounds;
            }



            // Many more vertices than spheres, vertices are outer loop
            for (int vertexIndex = 0; vertexIndex < projectedPositions.Length; vertexIndex+=1 )
            {
                var currentPosition0 = currentPositions[vertexIndex].Value;
                var projectedPosition0 = projectedPositions[vertexIndex].Value;
                var ray0 = projectedPosition0 - currentPosition0;

                Aabb rayBounds = new Aabb
                {
                    Min = math.min(currentPosition0, projectedPosition0)-eps,
                    Max = math.max(currentPosition0, projectedPosition0)+eps,
                };

                // brute force
                // we should be able to add some simple aabb tree over the triangles
                // but for the moment, just iterate over the entire array
                // for each polygon in the mesh, try a point triangle collision
                // for ( int x = 0; x<triangles.Length; ++x) 
                for (int hi = 0; hi < gridBounds.Length; ++hi)
                {
                    if (!rayBounds.Overlaps(gridBounds[hi]))
                    {
                        continue;
                    }

                    for (int hj = 0; hj < gridCounters[hi]; ++hj)
                    {
                        int x = gridIndices[gridBase[hi] + hj];

                        // quick check, is there any overlap
                        if (!rayBounds.Overlaps(boundingBoxes[x]))
                        {
                            continue;
                        }

                        int v0, v1, v2;

                        // there will be two triangles in the quad to test
                        v0 = triangles[x].v0;
                        v1 = triangles[x].v1;
                        v2 = triangles[x].v2;

                        // no self intersection
                        if ((v0 == vertexIndex) || (v1 == vertexIndex) || (v2 == vertexIndex))
                        {
                            continue;
                        }

                        // test swept movement against initial state
                        float s;
                        float t;

                        // Calculate the closest point on the triangle to the current particle position
                        var vc0 = currentPositions[v0].Value;
                        var vc1 = currentPositions[v1].Value;
                        var vc2 = currentPositions[v2].Value;
                        float3 tric0 = ClosestPointOnTriangle(vc0, vc1, vc2, currentPosition0, out s, out t);
                        float u = 1.0f - s - t;

                        // Find out where that point will be at the end of the frame
                        var vp0 = projectedPositions[v0].Value;
                        var vp1 = projectedPositions[v1].Value;
                        var vp2 = projectedPositions[v2].Value;
                        float3 trip0 = u * vp0 + s * vp1 + t * vp2;

                        // And get the ray for the triagle
                        float3 triRay = trip0 - tric0;
                        float3 diff = (tric0 - currentPosition0);

                        float3 pdiff = math.normalize(diff);
                        // project movement of the particle and the triangle onto the separating line
                        float dp0 = math.dot(ray0, pdiff);
                        float dpTri = math.dot(triRay, pdiff);

                        // If the two are approaching within the contact radius
                        // then we generate a speculative contact
                        float distance = (math.length(diff) - (dp0 - dpTri));

                        if (distance < 2.0f * (contactRadius))
                        {
                            float3 cross = math.cross(vc1 - vc0, vc2 - vc0);
                            float3 normal = math.normalize(cross);

                            // If the particle started on the backface of the triangle
                            // then we need to flip the values
                            float facing = 1.0f;
                            if (math.dot(currentPosition0 - vc0, normal) < 0.0f)
                            {
                                normal = -normal;
                                facing = -facing;
                            }

                            float push = contactRadius;
                            float planeDistance = math.dot(normal, vc0);
#if APPLY_CONSTRAINTS
                                // Add constraint for the particle
                                outGeneratedContacts.Add(new ClothCollisionContact
                                {
                                    ContactPlane = new float4 { xyz = normal, w = planeDistance + (push) },
                                    VertexIndex = vertexIndex
                                });

                                // push the triangle away from the point
                                AddTriangleConstraint(ref outGeneratedContacts, -normal, -planeDistance + push, v0, vc0, u, facing * (vc2 - vc1));
                                AddTriangleConstraint(ref outGeneratedContacts, -normal, -planeDistance + push, v1, vc1, s, facing * (vc0 - vc2));
                                AddTriangleConstraint(ref outGeneratedContacts, -normal, -planeDistance + push, v2, vc2, t, facing * (vc1 - vc0));
#else
                            // relative velocity in the frame

                            float value = 0.05f;
                            float kStiffness = 6.0f;


                            float rvel = (dp0 - dpTri);

                            rvel = math.max(rvel, 0.0f);

                            // Actual collision
                            if (distance < 0.0f)
                            {
                                value = rvel * 0.5f;
                            }
                            else
                            {
                                // surface contact
                                value = -math.min(-rvel, distance * kStiffness);
                            }

                            // The count applies 4 impulses, effectively dividing this value by 4
                            // so boost it back again
                            value *= 4.0f;


                            InternalImpulse imp = new InternalImpulse { Normal = normal, Value = value, VertexIndex = vertexIndex, Count = 1 };

                            impulses.Add(imp);
                            impulseCounts[vertexIndex] += 1;

                            imp.Value = -value * u;
                            imp.VertexIndex = v0;
                            impulses.Add(imp);
                            impulseCounts[v0] += 1;

                            imp.Value = -value * s;
                            imp.VertexIndex = v1;
                            impulses.Add(imp);
                            impulseCounts[v1] += 1;

                            imp.Value = -value * t;
                            imp.VertexIndex = v2;
                            impulses.Add(imp);
                            impulseCounts[v2] += 1;
#endif
                        }
                    }
                }
            }

#if !APPLY_CONSTRAINTS
            for (int i = 0; i<impulses.Length; ++i)
            {
                InternalImpulse imp = impulses[i];

                float3 ppos = projectedPositions[imp.VertexIndex].Value;

                float ic = impulseCounts[imp.VertexIndex];
                ppos += (imp.Value * imp.Normal)/ic;
                projectedPositions[imp.VertexIndex] = new ClothProjectedPosition { Value = ppos, };

            }
            impulses.Dispose();
#endif

            boundingBoxes.Dispose();
   
        }
    };


    protected override void OnCreate()
    {
        CreatePhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();

        base.OnCreate();
    }
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        CreatePhysicsWorldSystem.FinalJobHandle.Complete();

        PhysicsWorld world = CreatePhysicsWorldSystem.PhysicsWorld;

        var contactjob = new ClothPredictiveContactGenerationSystemJob()
        {
            physicsWorld = world,
            world = world.CollisionWorld,
            deltaTime = Time.fixedDeltaTime,
            contactRadius = 0.05f,
        };

 
        // TODO, this job can be done in parallel with the predictive contact job if contacts are two separate arrays which are merged later
        var clothjob = new ClothIntersectionContactJob()
        {
            physicsWorld = world,
            world = world.CollisionWorld,
            deltaTime = Time.fixedDeltaTime,
            contactRadius = 0.00625f,       // For self-interection, a small contact radius works best to avoid unstable lumpy cloth    
        };

        JobHandle ccHandle = contactjob.Schedule(this, inputDependencies);

        JobHandle rcHandle = clothjob.Schedule(this, ccHandle);

        return rcHandle;
    }
}