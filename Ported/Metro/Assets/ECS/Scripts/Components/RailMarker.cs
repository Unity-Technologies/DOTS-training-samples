using Unity.Entities;

public struct RailMarker : IComponentData
{
    public int Index;
    public RailMarkerType MarkerType;
}

public enum RailMarkerType
{
    PLATFORM_START,
    PLATFORM_END,
    ROUTE
}