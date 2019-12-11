using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Linq;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using System;

public class ClothComponent : IComponentData
{
    public Mesh Mesh;
    public float3 Gravity;
    public Material Material;
    public int FirstPinnedIndex;

    public NativeArray<float3> CurrentClothPosition;
    public NativeArray<float3> PreviousClothPosition;
    public NativeArray<float3> ClothNormals;

    public NativeArray<int2> Constraint1Indices;
    public NativeArray<float> Constraint1Lengths;

    public NativeArray<int2> Constraint2Indices;
    public NativeArray<float> Constraint2Lengths;
}

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
class ClothComponentSystem : ComponentSystem
{
    override protected void OnStartRunning()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            var vertices = cloth.Mesh.vertices;
            var normals = cloth.Mesh.normals;
            var indices = cloth.Mesh.triangles;

            cloth.CurrentClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.PreviousClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.ClothNormals = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            var pinned = new bool[vertices.Length];
            var lastPinned = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                cloth.PreviousClothPosition[i] =
                cloth.CurrentClothPosition[i] = vertices[i];

                cloth.ClothNormals[i] = normals[i];

                if (normals[i].y > .9f && vertices[i].y > .3f)
                {
                    pinned[i] = true;
                }
                else
                {
                    if (lastPinned != i)
                    {
                        { var t = cloth.PreviousClothPosition[lastPinned]; cloth.PreviousClothPosition[lastPinned] = cloth.PreviousClothPosition[i]; cloth.PreviousClothPosition[i] = t; }
                        { var t = cloth.CurrentClothPosition[lastPinned]; cloth.CurrentClothPosition[lastPinned] = cloth.CurrentClothPosition[i]; cloth.CurrentClothPosition[i] = t; }
                        { var t = cloth.ClothNormals[lastPinned]; cloth.ClothNormals[lastPinned] = cloth.ClothNormals[i]; cloth.ClothNormals[i] = t; }
                        { var t = pinned[lastPinned]; pinned[lastPinned] = pinned[i]; pinned[i] = t; }
                        { var t = vertices[lastPinned]; vertices[lastPinned] = vertices[i]; vertices[i] = t; }
                        { var t = normals[lastPinned]; normals[lastPinned] = normals[i]; normals[i] = t; }

                        // horrible hack
                        for (int j = 0; j < indices.Length; j++)
                        {
                            if (indices[j] == i)
                                indices[j] = lastPinned;
                            else if (indices[j] == lastPinned)
                                indices[j] = i;
                        }
                        lastPinned++;
                    }
                }
            }

            cloth.FirstPinnedIndex = lastPinned;


            cloth.Mesh.triangles = indices;
            cloth.Mesh.vertices = vertices;
            cloth.Mesh.normals = normals;

            var constraint1Lookup = new HashSet<int2>();
            var constraint2Lookup = new HashSet<int2>();
            for (int i = 0; i < indices.Length; i += 3)
            {
                var index0 = indices[i + 0];
                var index1 = indices[i + 1];
                var index2 = indices[i + 2];

                int2 constraint0;
                int2 constraint1;
                int2 constraint2;

                if (index0 > index1) constraint0 = new int2(index0, index1); else constraint0 = new int2(index1, index0);
                if (index1 > index2) constraint1 = new int2(index1, index2); else constraint1 = new int2(index2, index1);
                if (index2 > index0) constraint2 = new int2(index2, index0); else constraint2 = new int2(index0, index2);

                if (!pinned[index0] || !pinned[index1])
                {
                    if (pinned[index0])
                        constraint1Lookup.Add(new int2(index1, index0));
                    else
                    if (pinned[index1])
                        constraint1Lookup.Add(new int2(index0, index1));
                    else
                        constraint2Lookup.Add(constraint0);
                }
                if (!pinned[index1] || !pinned[index2])
                {
                    if (pinned[index1])
                        constraint1Lookup.Add(new int2(index2, index1));
                    else
                    if (pinned[index2])
                        constraint1Lookup.Add(new int2(index1, index2));
                    else
                        constraint2Lookup.Add(constraint1);
                }
                if (!pinned[index0] || !pinned[index2])
                {
                    if (pinned[index2])
                        constraint1Lookup.Add(new int2(index0, index2));
                    else
                    if (pinned[index0])
                        constraint1Lookup.Add(new int2(index2, index0));
                    else
                        constraint2Lookup.Add(constraint2);
                }
            }


            cloth.Constraint1Indices = new NativeArray<int2>(constraint1Lookup.ToArray(), Allocator.Persistent);
            cloth.Constraint2Indices = new NativeArray<int2>(constraint2Lookup.ToArray(), Allocator.Persistent);

            cloth.Constraint1Lengths = new NativeArray<float>(constraint1Lookup.Count, Allocator.Persistent);
            for (int i = 0; i < cloth.Constraint1Indices.Length; i++)
            {
                var constraint = cloth.Constraint1Indices[i];

                var vertex0 = cloth.PreviousClothPosition[constraint.x];
                var vertex1 = cloth.PreviousClothPosition[constraint.y];

                cloth.Constraint1Lengths[i] = math.length(vertex1 - vertex0);
            }

            cloth.Constraint2Lengths = new NativeArray<float>(constraint2Lookup.Count, Allocator.Persistent);
            for (int i = 0; i < cloth.Constraint2Indices.Length; i++)
            {
                var constraint = cloth.Constraint2Indices[i];

                var vertex0 = cloth.PreviousClothPosition[constraint.x];
                var vertex1 = cloth.PreviousClothPosition[constraint.y];

                cloth.Constraint2Lengths[i] = math.length(vertex1 - vertex0);
            }
        });
    }

    override protected void OnDestroy()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            if (cloth.CurrentClothPosition.IsCreated) cloth.CurrentClothPosition.Dispose();
            if (cloth.PreviousClothPosition.IsCreated) cloth.PreviousClothPosition.Dispose();
            if (cloth.ClothNormals.IsCreated) cloth.ClothNormals.Dispose();

            if (cloth.Constraint1Indices.IsCreated) cloth.Constraint1Indices.Dispose();
            if (cloth.Constraint1Lengths.IsCreated) cloth.Constraint1Lengths.Dispose();

            if (cloth.Constraint2Indices.IsCreated) cloth.Constraint2Indices.Dispose();
            if (cloth.Constraint2Lengths.IsCreated) cloth.Constraint2Lengths.Dispose();
        });
    }


    override protected void OnUpdate()
    {
        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var constraint1Job = new Constraint1Job
            {
                vertices = cloth.CurrentClothPosition,
                constraintIndices = cloth.Constraint1Indices,
                constraintLengths = cloth.Constraint1Lengths
            };

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
            };

            var collisionJob = new CollisionMeshJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                localToWorld = localToWorld.Value,
                worldToLocal = math.inverse(localToWorld.Value)
            };

            var constraint1Handle = constraint1Job.Schedule();
            var constraint2Handle = constraint2Job.Schedule(constraint1Handle);
            var meshHandle = meshJob.Schedule(cloth.FirstPinnedIndex, 128, constraint2Handle);
            var collisionhHandle = collisionJob.Schedule(cloth.FirstPinnedIndex, 128, meshHandle);
            collisionhHandle.Complete();

            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);

            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        });
    }
}