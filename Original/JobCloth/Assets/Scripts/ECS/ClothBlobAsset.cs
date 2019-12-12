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

public struct ClothBlobAsset
{
    public int FirstPinnedIndex;

    public BlobArray<int2> Constraint1Indices;
    public BlobArray<float> Constraint1Lengths;

    public BlobArray<int2> Constraint2Indices;
    public BlobArray<float> Constraint2Lengths;
}

public class ClothBlobAssetUtility
{
    public static BlobAssetReference<ClothBlobAsset> CreateFromMesh(Mesh mesh)
    {
        var vertices    = mesh.vertices;
        var normals     = mesh.normals;
        var indices     = mesh.triangles;

        var pinned = new bool[vertices.Length];
        var lastPinned = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (normals[i].y > .9f && vertices[i].y > .3f)
            {
                pinned[i] = true;
            }
            else
            {
                if (lastPinned != i)
                {
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

        
        mesh.triangles = indices;
        mesh.vertices = vertices;
        mesh.normals = normals;

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


        using (var builder = new BlobBuilder(Allocator.Temp))
        {
            ref var root = ref builder.ConstructRoot<ClothBlobAsset>();

            var constraint1Indices = builder.Construct(ref root.Constraint1Indices, constraint1Lookup.ToArray());
            var constraint2Indices = builder.Construct(ref root.Constraint2Indices, constraint2Lookup.ToArray());

            var constraint1Lengths = builder.Allocate(ref root.Constraint1Lengths, constraint1Lookup.Count);
            var constraint2Lengths = builder.Allocate(ref root.Constraint2Lengths, constraint2Lookup.Count);

            for (int i = 0; i < constraint1Indices.Length; i++)
            {
                var constraint = constraint1Indices[i];

                var vertex0 = (float3)vertices[constraint.x];
                var vertex1 = (float3)vertices[constraint.y];

                constraint1Lengths[i] = -math.length(vertex1 - vertex0);
            }

            for (int i = 0; i < constraint2Indices.Length; i++)
            {
                var constraint = constraint2Indices[i];

                var vertex0 = (float3)vertices[constraint.x];
                var vertex1 = (float3)vertices[constraint.y];

                constraint2Lengths[i] = math.length(vertex1 - vertex0) * -0.5f;
            }

            root.FirstPinnedIndex = lastPinned;

            return builder.CreateBlobAssetReference<ClothBlobAsset>(Allocator.Persistent);
        }
    }
}