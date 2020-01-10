using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Rendering;

[UpdateAfter(typeof(ClothSimRenderMesh))]
public class ClothSimRenderMesh : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((RenderMesh renderMesh,
                                         ref DynamicBuffer<VertexStateCurrentElement> currentVertexState) =>
        {
            // jiv - RenderMesh is a shared component. We happen to know that there's a 1-1
            //       relationship between entities and RenderMesh instances, so it's not
            //       actually causing an error, but updating the mesh vertices per instance
            //       seems incorrect.
            renderMesh.mesh.SetVertices(currentVertexState.Reinterpret<Vector3>().AsNativeArray());
        }).Run();

        return inputDeps;
    }
}
