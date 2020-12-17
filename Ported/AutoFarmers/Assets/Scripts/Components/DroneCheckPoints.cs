using Unity.Entities;
using Unity.Mathematics;

public struct DroneCheckPoints : IComponentData 
{
    public float2 storePosition;
    public float3 destination;
    public float hoverHeight;
}