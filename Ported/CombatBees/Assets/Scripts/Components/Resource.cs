using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public float3 boundsExtents;
    public float3 velocity;
}

public struct ResourceCarried : IComponentData, IEnableableComponent
{
    
}

public struct ResourceDropped : IComponentData, IEnableableComponent
{
    
}