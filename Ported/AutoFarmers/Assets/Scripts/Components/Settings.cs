using Unity.Entities;
using Unity.Mathematics;

public struct Settings : IComponentData
{
    public int2   GridSize;
    public float3 CameraOffset;
    public float  CameraDamping;
}

public enum TileStates
{
    Empty,
    Store,
    Rock,
    Tilled,
    Seeded,
    Grown
}

public struct TileState : IBufferElementData
{
    public TileStates Value;
}