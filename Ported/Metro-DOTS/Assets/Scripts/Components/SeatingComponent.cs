using Unity.Entities;
using Unity.Mathematics;

public struct SeatingComponentElement : IBufferElementData
{
    public float3 SeatPosition;
    public bool Occupied;
}