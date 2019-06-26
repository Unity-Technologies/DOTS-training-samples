using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

[Serializable]
[InternalBufferCapacity(1024)]
public struct ClothProjectedPosition : IBufferElementData
{
    public float3 Value;
}

[Serializable]
[InternalBufferCapacity(4)]
public struct ClothCurrentPosition : IBufferElementData
{
    public float3 Value;
}

[Serializable]
[InternalBufferCapacity(4)]
public struct ClothPreviousPosition : IBufferElementData
{
    public float3 Value;
}

[Serializable]
[InternalBufferCapacity(4)]
public struct ClothDistanceConstraint : IBufferElementData
{
    public int VertexA;
    public int VertexB;
    public float RestLengthSqr;
}

[Serializable]
[InternalBufferCapacity(8)]
public struct ClothSphereCollider : IBufferElementData
{
    public float3 LocalCenter;
    public float  Radius;
}

[Serializable]
[InternalBufferCapacity(8)]
public struct ClothCapsuleCollider : IBufferElementData
{
    public float3 LocalVertexA;
    public float  RadiusA;
    public float3 LocalVertexB;
    public float  RadiusB;

}

[Serializable]
[InternalBufferCapacity(8)]
public struct ClothPlaneCollider : IBufferElementData
{
    public float3 LocalNormal;
    public float LocalOffset;
}

[Serializable]
[InternalBufferCapacity(64)]
public struct ClothCollisionContact : IBufferElementData
{
    public float4 ContactPlane;
    public int VertexIndex;
}

[Serializable]
[InternalBufferCapacity(4)]
public struct ClothPositionOrigin : IBufferElementData
{
    public float3 Origin;
}

[Serializable]
[InternalBufferCapacity(4)]
public struct ClothPinWeight : IBufferElementData
{ 
    public float InvPinWeight;
}

[Serializable]
public struct ClothSourceMeshData : IComponentData
{
    public GCHandle SrcMeshHandle;
}

[Serializable]
public struct ClothTotalTime : IComponentData
{
    public float TotalTime;
}

[Serializable]
public struct ClothTimestepData : IComponentData
{
    public float FixedTimestep;
    public int   IterationCount;
}


[Serializable]
public struct ClothWorldToLocal : IComponentData
{
    public float4x4 Value;
}

// Hierarchical only components

public struct ClothHierarchyDepth : ISharedComponentData
{
    public int Level;
}

public struct ClothIndexInMesh : IComponentData
{
    public int OriginalIndex;
}

[Serializable]
[InternalBufferCapacity(4)]
public unsafe struct ClothHierarchicalParentIndexAndWeights : IBufferElementData
{
    public fixed int   ParentIndex[8];
    public fixed float WeightValue[8];
}

public struct ClothHierarchyParentEntity : IComponentData
{
    public Entity Parent;
}