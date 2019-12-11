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

    public NativeArray<float3> CurrentClothPosition;
    public NativeArray<float3> PreviousClothPosition;
    public NativeArray<float3> Forces;
    public NativeArray<float3> ClothNormals;

    public BlobAssetReference<ClothBlobAsset> constraints;
}

[BurstCompile]
[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
class ClothComponentSystem : ComponentSystem
{
    override protected void OnStartRunning()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            cloth.constraints = ClothBlobAssetUtility.CreateFromMesh(cloth.Mesh);

            var vertices = cloth.Mesh.vertices;
            var normals = cloth.Mesh.normals;
            var indices = cloth.Mesh.triangles;

            cloth.CurrentClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.PreviousClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.Forces = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.ClothNormals = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            
            for (int i = 0; i < vertices.Length; i++)
            {
                cloth.PreviousClothPosition[i] =
                cloth.CurrentClothPosition[i] = vertices[i];
                cloth.Forces[i] = float3.zero;

                cloth.ClothNormals[i] = normals[i];
            }
        });
    }

    override protected void OnDestroy()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            if (cloth.CurrentClothPosition.IsCreated) cloth.CurrentClothPosition.Dispose();
            if (cloth.PreviousClothPosition.IsCreated) cloth.PreviousClothPosition.Dispose();
            if (cloth.Forces.IsCreated) cloth.Forces.Dispose();
            if (cloth.ClothNormals.IsCreated) cloth.ClothNormals.Dispose();
        });
    }


    override protected void OnUpdate()
    {
        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            
            
        });
    }
}