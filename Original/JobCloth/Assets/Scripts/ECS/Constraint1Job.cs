using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

[BurstCompile]
public unsafe struct Constraint1Job : IJob
{
    public NativeArray<float3> vertices;
    [ReadOnly] public NativeArray<int2> constraintIndices;
    [ReadOnly] public NativeArray<float> constraintLengths;

    public void Execute()
    {
        var indexCount = constraintIndices.Length;
        float3* verticesPtr = (float3*)NativeArrayUnsafeUtility.GetUnsafePtr(vertices);
        int2* constraintIndicesPtr = (int2*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(constraintIndices);
        float* constraintLengthsPtr = (float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(constraintLengths);
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
}


[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(ClothComponentSystem))]
public unsafe class Constraint1_System : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var vertices = cloth.CurrentClothPosition;
            var constraintIndices = cloth.Constraint1Indices;
            var constraintLengths = cloth.Constraint1Lengths;
            var forces = cloth.Forces;

            //Execute
            {
                var indexCount = constraintIndices.Length;

                if (vertices == null || !vertices.IsCreated)
                    Console.WriteLine("Here");


                float3* verticesPtr = (float3*)NativeArrayUnsafeUtility.GetUnsafePtr(vertices);
                int2* constraintIndicesPtr = (int2*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(constraintIndices);
                float* constraintLengthsPtr = (float*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(constraintLengths);
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

            var constraint2Job = new Constraint2Job
            {
                vertices = cloth.CurrentClothPosition,
                constraintIndices = cloth.Constraint2Indices,
                constraintLengths = cloth.Constraint2Lengths
            };

            var meshJob = new AccumulateForcesJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                gravity = cloth.Gravity,
                forces = forces
            };

            var collisionJob = new CollisionMeshJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                localToWorld = localToWorld.Value,
                worldToLocal = math.inverse(localToWorld.Value)
            };

            var constraint2Handle = constraint2Job.Schedule();
            var meshHandle = meshJob.Schedule(cloth.FirstPinnedIndex, 128, constraint2Handle);
            var collisionhHandle = collisionJob.Schedule(cloth.FirstPinnedIndex, 128, meshHandle);
            collisionhHandle.Complete();

            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        });
    }
}

