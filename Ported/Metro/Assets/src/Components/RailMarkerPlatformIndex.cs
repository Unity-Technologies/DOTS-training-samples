using Unity.Entities;

public struct RailMarkerPlatformIndex : IBufferElementData
{
    public int Value;

    public static implicit operator int(RailMarkerPlatformIndex v) => v.Value;
    public static implicit operator RailMarkerPlatformIndex(int v) => new RailMarkerPlatformIndex { Value = v };
}
