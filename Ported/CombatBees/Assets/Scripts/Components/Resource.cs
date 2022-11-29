using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public float3 boundsExtents;
}

public struct ResourceCarried : IComponentData
{
    
}

public struct ResourceDropped : IComponentData
{
    
}