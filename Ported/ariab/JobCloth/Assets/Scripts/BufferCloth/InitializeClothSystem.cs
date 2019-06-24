using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//[UpdateInGroup(typeof(InitializationSystemGroup))]
public unsafe class InitializeClothSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Transform transform, MeshFilter meshFilter, VertexClothGarment garment) =>
        {
            var mesh = meshFilter.mesh;
            var vertexCount = mesh.vertexCount;
            
            var archetype = DstEntityManager.CreateArchetype( 
                typeof(ClothProjectedPosition), 
                typeof(ClothCurrentPosition), 
                typeof(ClothPreviousPosition), 
                typeof(ClothDistanceConstraint), 
                typeof(ClothPositionOrigin), 
                typeof(ClothPinWeight),
                typeof(ClothTotalTime),
                typeof(ClothTimestepData),
                typeof(ClothSourceMeshData),
                typeof(ClothSphereCollider),
                typeof(ClothCapsuleCollider),
                typeof(ClothPlaneCollider),
                typeof(ClothCollisionContact),
                typeof(ClothWorldToLocal));
            var entity = DstEntityManager.CreateEntity(archetype);
            
            // Add reference to source mesh data and set it as read/write
            mesh.MarkDynamic();

            var meshHandle = GCHandle.Alloc(mesh, GCHandleType.Pinned);
            var srcMeshData = new ClothSourceMeshData
            {
                SrcMeshHandle = meshHandle
            };
            DstEntityManager.SetComponentData(entity, srcMeshData);
            DstEntityManager.SetComponentData(entity, new ClothTotalTime { TotalTime = 0.0f });
            DstEntityManager.SetComponentData(entity, new ClothTimestepData {FixedTimestep = 1.0f / 60.0f, IterationCount = 0});
            DstEntityManager.SetComponentData(entity, new ClothWorldToLocal { Value = transform.worldToLocalMatrix });

            // Copy initial vert data to buffer
            var projectedPositionBuffer = DstEntityManager.GetBuffer<ClothProjectedPosition>(entity);
            projectedPositionBuffer.Reserve(vertexCount);
            
            var currentPositionBuffer = DstEntityManager.GetBuffer<ClothCurrentPosition>(entity);
            currentPositionBuffer.Reserve(vertexCount);
            
            var previousPositionBuffer = DstEntityManager.GetBuffer<ClothPreviousPosition>(entity);
            previousPositionBuffer.Reserve(vertexCount);
            
            var originPositionBuffer = DstEntityManager.GetBuffer<ClothPositionOrigin>(entity);
            originPositionBuffer.Reserve(vertexCount);

            fixed (Vector3* positions = mesh.vertices)
            {
                var currentPositionsAsNativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothCurrentPosition>((float3*) positions, vertexCount, Allocator.Invalid);
                var projectedPositionsAsNativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothProjectedPosition>((float3*) positions, vertexCount, Allocator.Invalid);
                var previousPositionsAsNativeArray  = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothPreviousPosition>((float3*) positions, vertexCount, Allocator.Invalid);
                var originPositionsAsNativeArray  = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ClothPositionOrigin>((float3*) positions, vertexCount, Allocator.Invalid);
                
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref currentPositionsAsNativeArray, AtomicSafetyHandle.Create());
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref projectedPositionsAsNativeArray, AtomicSafetyHandle.Create());
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref previousPositionsAsNativeArray, AtomicSafetyHandle.Create());
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref originPositionsAsNativeArray, AtomicSafetyHandle.Create());
#endif
                currentPositionBuffer.CopyFrom(currentPositionsAsNativeArray);
                projectedPositionBuffer.CopyFrom(projectedPositionsAsNativeArray);
                previousPositionBuffer.CopyFrom(previousPositionsAsNativeArray);
                originPositionBuffer.CopyFrom(originPositionsAsNativeArray);
            }
            
            // Add constraints to the entity
            // todo: no garbage
            var barLookup = new HashSet<Vector2Int>();
            var triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2Int pair = new Vector2Int
                    {
                        x = triangles[i + j],
                        y = triangles[i + (j + 1) % 3]
                    };
                    
                    if (pair.x > pair.y)
                    {
                        var newY = pair.x;
                        pair.x = pair.y;
                        pair.y = newY;
                    }
        
                    if (barLookup.Contains(pair) == false)
                    {
                        barLookup.Add(pair);
                    }
                }
            }
        
            // todo: no garbage
            var barList = new List<Vector2Int>(barLookup);
            var constraintCount = barList.Count;
            
            var constraintsBuffer = DstEntityManager.GetBuffer<ClothDistanceConstraint>(entity);
            constraintsBuffer.Reserve(constraintCount);

            var vertices = mesh.vertices;
            for (int i = 0; i < constraintCount; ++i)
            {
                Vector3 p1 = vertices[barList[i].x];
                Vector3 p2 = vertices[barList[i].y];
                
                constraintsBuffer.Add(new ClothDistanceConstraint
                {
                    RestLengthSqr = (p2 - p1).sqrMagnitude,
                    VertexA = barList[i].x,
                    VertexB = barList[i].y
                });
            }
            
            // Add pin weights
            var pinWeightBuffer = DstEntityManager.GetBuffer<ClothPinWeight>(entity);
            pinWeightBuffer.Reserve(vertexCount);

            var normals = mesh.normals;
            for (int i = 0; i < vertexCount; ++i)
            {
                if (normals[i].y > .9f && vertices[i].y > .3f)
                    pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 0.0f});
                else
                    pinWeightBuffer.Add(new ClothPinWeight {InvPinWeight = 1.0f});
            }
            
            // Sphere Colliders
            var sphereColliders = DstEntityManager.GetBuffer<ClothSphereCollider>(entity);
            sphereColliders.Reserve(garment.m_SphereColliders.Length);
            for (int i = 0; i < garment.m_SphereColliders.Length; ++i)
            {
                var sphere = garment.m_SphereColliders[i];
                sphereColliders.Add(new ClothSphereCollider{LocalCenter = sphere.xyz, Radius = sphere.w});
            }
            
            // todo Capsule Colliders
            //var capsuleColliders = DstEntityManager.GetBuffer<ClothCapsuleCollider>(entity);
            //capsuleColliders.Reserve(garment.m_CapsuleColliders.Length);
            //for (int i = 0; i < garment.m_CapsuleColliders.Length; ++i)
            //{
            //    var capsule = garment.m_CapsuleColliders[i];
            //    var vertA = capsule.center + ((capsule.height - capsule.radius) * capsule.direction);
            //    
            //    capsuleColliders.Add(new ClothCapsuleCollider{LocalCenter = capsule., Radius = collider.radius});
            //}
            
            // Plane Colliders
            var planeColliders = DstEntityManager.GetBuffer<ClothPlaneCollider>(entity);
            planeColliders.Reserve(garment.m_PlaneColliders.Length);
            for (int i = 0; i < garment.m_PlaneColliders.Length; ++i)
            {
                var plane = garment.m_PlaneColliders[i];
                planeColliders.Add(new ClothPlaneCollider{LocalNormal = plane.xyz, LocalOffset = plane.w});
            }
        });
    }
}