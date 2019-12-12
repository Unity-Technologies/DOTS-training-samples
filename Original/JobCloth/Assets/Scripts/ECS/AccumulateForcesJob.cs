using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(Constraint2_System))]
public unsafe class AccumulateForces_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<CurrentVertex> vertices, ref DynamicBuffer<PreviousVertex> oldVertices, in ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            var gravity     = cloth.Gravity;

            var verticesPtr = (float3*)vertices.GetUnsafePtr();
            var oldVerticesPtr = (float3*)oldVertices.GetUnsafePtr();

            var firstPinnedIndex = cloth.constraints.Value.FirstPinnedIndex;
            for (int i = 0; i < firstPinnedIndex; i++)
            {
                float3 oldVert  = oldVerticesPtr[i];
                float3 vert     = verticesPtr[i];
                oldVerticesPtr[i]   = vert;
                verticesPtr[i]      = (vert + vert - oldVert + gravity);
            }
        }).Schedule(inputDeps);
    }
}