using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
unsafe struct AccumulateForcesJob : IJobParallelFor
{
    public NativeArray<float3> vertices;
    public NativeArray<float3> oldVertices;
    public NativeArray<float3> forces;
    public float3 gravity;

    public void Execute(int i)
    {
        float3 oldVert = oldVertices[i];
        float3 vert = vertices[i];
        float3 startPos = vert;

        vert += (vert - oldVert + forces[i]);

        vertices[i] = vert;
        oldVertices[i] = startPos;
        forces[i] = gravity;
    }
}

[BurstCompile]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(Constraint2_System))]
public class AccumulateForces_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var vertices = cloth.CurrentClothPosition;
            var oldVertices = cloth.PreviousClothPosition;
            var gravity = cloth.Gravity;
            var forces = cloth.Forces;

            //Execute
            for (int i = 0; i < vertices.Length; ++i)
            {
                float3 oldVert = oldVertices[i];
                float3 vert = vertices[i];
                float3 startPos = vert;

                vert += (vert - oldVert + forces[i]);

                vertices[i] = vert;
                oldVertices[i] = startPos;
                forces[i] = gravity;
            }

            var collisionJob = new CollisionMeshJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                localToWorld = localToWorld.Value,
                worldToLocal = math.inverse(localToWorld.Value)
            };

            var collisionhHandle = collisionJob.Schedule(cloth.FirstPinnedIndex, 128);
            collisionhHandle.Complete();

            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        });
    }
}