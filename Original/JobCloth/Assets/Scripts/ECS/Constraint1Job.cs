using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

[BurstCompile]
public unsafe struct Constraint1Job : IJob
{
    public NativeArray<float3> vertices;
    [ReadOnly] public BlobAssetReference<ClothBlobAsset> constraints;

    public void Execute()
    {
        float3* verticesPtr = (float3*)NativeArrayUnsafeUtility.GetUnsafePtr(vertices);
        ref var constraintIndices = ref constraints.Value.Constraint1Indices;
        ref var constraintLengths = ref constraints.Value.Constraint1Lengths;
        var indexCount = constraintIndices.Length;
        for (int i = 0; i < indexCount; i++)
        {
            int2 pair = constraintIndices[i];

            float3 p1 = verticesPtr[pair.x];
            float3 p2 = verticesPtr[pair.y];

            var delta = p2 - p1;
            var length = math.length(delta);
            var extra = (length - constraintLengths[i]);
            var dir = delta / length;
            var offset = extra * dir;
            verticesPtr[pair.x] = p1 + offset;
        }
    }
}


[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(ClothComponentSystem))]
public unsafe class Constraint1_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var vertices = cloth.CurrentClothPosition;
            ref var constraintIndices = ref cloth.constraints.Value.Constraint1Indices;
            ref var constraintLengths = ref cloth.constraints.Value.Constraint1Lengths;
            var forces = cloth.Forces;

            //Execute
            {
                var indexCount = constraintIndices.Length;

                float3* verticesPtr = (float3*)NativeArrayUnsafeUtility.GetUnsafePtr(vertices);
                int2* constraintIndicesPtr = (int2*)constraintIndices.GetUnsafePtr();
                float* constraintLengthsPtr = (float*)constraintLengths.GetUnsafePtr();
                for (int i = 0; i < indexCount; i++)
                {
                    int2 pair = constraintIndicesPtr[i];

                    float3 p1 = verticesPtr[pair.x];
                    float3 p2 = verticesPtr[pair.y];

                    var delta = p2 - p1;
                    var length = math.length(delta);
                    var extra = (length - constraintLengthsPtr[i]);
                    var dir = delta / length;
                    var offset = extra * dir;
                    verticesPtr[pair.x] = p1 + offset;
                }
            }            
        }).Run(); ;

        return inputDeps;
    }
}

