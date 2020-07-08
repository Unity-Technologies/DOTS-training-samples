using Unity.Entities;
using Unity.Mathematics;

public struct GridHeight : IBufferElementData
{
    public float Height;
}

public struct GridOccupied : IBufferElementData
{
    public bool Occupied; // true if there is a cannon.
}

public struct GridTag : IComponentData
{}