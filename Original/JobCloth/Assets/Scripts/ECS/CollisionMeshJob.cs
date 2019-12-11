
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
unsafe struct CollisionMeshJob : IJobParallelFor
{
    public NativeArray<float3> vertices;
    public NativeArray<float3> oldVertices;
    public float4x4 localToWorld;
    public float4x4 worldToLocal;

    public void Execute(int i)
    {
        float3 oldVert = oldVertices[i];
        float3 vert = vertices[i];

        float3 worldPos = math.mul(localToWorld, new float4(vert, 1)).xyz;

        if (worldPos.y < 0f)
        {
            float3 oldWorldPos = math.mul(localToWorld, new float4(oldVert, 1)).xyz;
            oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
            worldPos.y = 0f;
            vert = math.mul(worldToLocal, new float4(worldPos, 1)).xyz;
            oldVert = math.mul(worldToLocal, new float4(oldWorldPos, 1)).xyz;
        }

        vertices[i] = vert;
        oldVertices[i] = oldVert;
    }
}

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(AccumulateForces_System))]
public class CollisionMesh_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var collisionJob = new CollisionMeshJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                localToWorld = localToWorld.Value,
                worldToLocal = math.inverse(localToWorld.Value)
            };

            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        }).Run();

        return inputDeps;
    }
}