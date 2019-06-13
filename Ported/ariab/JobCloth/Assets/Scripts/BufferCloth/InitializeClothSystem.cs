using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

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
        });
    }
}