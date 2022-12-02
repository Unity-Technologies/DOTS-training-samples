using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public Entity ownerBee;
    public float3 boundsExtents;
    public float3 velocity;

    public bool droppedEnabled;
    public bool carriedEnabled;

    public float3 ownerVelocity;
    public float3 ownerPosition;
}

public struct ResourceCarried : IComponentData, IEnableableComponent
{
    
}

public struct ResourceDropped : IComponentData, IEnableableComponent
{
    
}

public struct ResourceHiveReached : IComponentData, IEnableableComponent
{
    
}