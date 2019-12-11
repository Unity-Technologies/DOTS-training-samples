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
    public float3 Gravity;

    public NativeArray<float3> CurrentClothPosition;
    public NativeArray<float3> PreviousClothPosition;
    public NativeArray<float3> Forces;
    public NativeArray<float3> ClothNormals;

    public BlobAssetReference<ClothBlobAsset> constraints;
}

public class ClothRenderComponent : IComponentData
{
    public Mesh     Mesh;
    public Material Material;
}