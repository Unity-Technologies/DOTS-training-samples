using Unity.Entities;

public struct RailMarker : IComponentData
{
    public Entity Line;
    public int Index;
}

public enum RailMarkerType
{
    PLATFORM_START,
    PLATFORM_END,
    ROUTE
}

public struct PlatformStartTag : IComponentData {}
public struct PlatformEndTag : IComponentData {}
public struct RouteTag : IComponentData {}