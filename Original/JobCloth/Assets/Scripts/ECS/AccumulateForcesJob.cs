using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(Constraint2_System))]
public unsafe class AccumulateForces_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<Force> forces, ref DynamicBuffer<CurrentVertex> vertices, ref DynamicBuffer<PreviousVertex> oldVertices, in ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            var gravity     = cloth.Gravity;

            var forcesPtr = (float3*)forces.GetUnsafePtr();
            var verticesPtr = (float3*)vertices.GetUnsafePtr();
            var oldVerticesPtr = (float3*)oldVertices.GetUnsafePtr();

            var firstPinnedIndex = cloth.constraints.Value.FirstPinnedIndex;
            //Execute
            for (int i = 0; i < firstPinnedIndex; ++i)
            {
                float3 oldVert  = oldVerticesPtr[i];
                float3 vert     = verticesPtr[i];
                float3 startPos = vert;

                vert += (vert - oldVert + forcesPtr[i]);

                oldVerticesPtr[i]   = vert;
                oldVertices[i]      = startPos;
                forcesPtr[i]        = gravity;
            }
        }).Schedule(inputDeps);
    }
}