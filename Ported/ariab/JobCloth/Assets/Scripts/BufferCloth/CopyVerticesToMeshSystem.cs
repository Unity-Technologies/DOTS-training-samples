using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

// See ClothSolverSystemGroup
[DisableAutoCreation]
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