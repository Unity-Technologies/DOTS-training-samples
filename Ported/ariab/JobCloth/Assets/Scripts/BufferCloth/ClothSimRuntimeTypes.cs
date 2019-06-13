using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;

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
