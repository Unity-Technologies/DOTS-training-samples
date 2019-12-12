using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public unsafe class Constraint1_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<CurrentVertex> vertices, in ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            //Dynamic buffer and BlobArray don't have GetUnsafeReadOnlyPtr, so we grab
            //the ptr instead.
            ref var constraintIndices = ref cloth.constraints.Value.Constraint1Indices;
            ref var constraintLengths = ref cloth.constraints.Value.Constraint1Lengths;

            var constraintIndicesPtr = (int2*)constraintIndices.GetUnsafePtr();
            var constraintLengthsPtr = (float*)constraintLengths.GetUnsafePtr();
            var verticesPtr          = (float3*)vertices.GetUnsafePtr();

            var indexCount = constraintIndices.Length;
            for (int i = 0; i < indexCount; i++)
            {
                int2 pair = constraintIndicesPtr[i];

                float3 p1 = verticesPtr[pair.x];
                float3 p2 = verticesPtr[pair.y];

                var delta = p2 - p1;
                var length = math.rsqrt(math.lengthsq(delta));
                var offset = (constraintLengthsPtr[i] * length) * delta;

                verticesPtr[pair.x] = p2 + offset;
            }
        }).Schedule(inputDeps);
    }
}

