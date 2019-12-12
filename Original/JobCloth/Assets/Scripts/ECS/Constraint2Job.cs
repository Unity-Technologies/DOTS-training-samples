using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(Constraint1_System))]
public unsafe class Constraint2_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<CurrentVertex> vertices, in ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            ref var constraintIndices = ref cloth.constraints.Value.Constraint2Indices;
            ref var constraintLengths = ref cloth.constraints.Value.Constraint2Lengths;

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
                var offset = ((constraintLengthsPtr[i] * length) + 0.5f) * delta;

                verticesPtr[pair.x] = p1 + offset;
                verticesPtr[pair.y] = p2 - offset;
            }
        }).Schedule(inputDeps);
    }
}



