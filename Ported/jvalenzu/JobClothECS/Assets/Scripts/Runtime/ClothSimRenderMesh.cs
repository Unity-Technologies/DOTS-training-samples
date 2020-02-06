using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Tiny.Rendering;

// jiv fixme - conditionalize on UNITY_DOTSPLAYER

public class ClothSimRenderMesh : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((SimpleMeshRenderData renderMesh,
                                 ref DynamicBuffer<VertexStateCurrentElement> currentVertexState) =>
        {
            ref SimpleMeshData simpleMeshData = ref renderMesh.Mesh.Value;
            NativeArray<float3> newVertexData = currentVertexState.Reinterpret<float3>().AsNativeArray();
            for (int i=0,n=simpleMeshData.Vertices.Length; i<n; ++i)
            {
                simpleMeshData.Vertices[i].Position = newVertexData[i];
            }
        }).Schedule(inputDeps);
    }
}
