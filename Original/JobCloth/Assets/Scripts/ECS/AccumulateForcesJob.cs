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

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(Constraint2_System))]
public class AccumulateForces_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
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
        }).Run();

        return inputDeps;
    }
}