using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public unsafe class CopyVerticesToMeshSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //todo: no
        EntityManager.CompleteAllJobs();
        
        Entities.ForEach((ref ClothSourceMeshData srcData, DynamicBuffer<ClothCurrentPosition> currentPositionBuffer) =>
        {
            var mesh = srcData.SrcMeshHandle.Target as Mesh;
            
            var srcPtr = currentPositionBuffer.GetUnsafePtr();
            var dstArray = mesh.vertices;
            fixed (Vector3* dstPtr = &dstArray[0])
            {
                UnsafeUtility.MemCpy(dstPtr, srcPtr, sizeof(Vector3)*currentPositionBuffer.Length);
            }
            
            mesh.vertices = dstArray;
        });
    }
}