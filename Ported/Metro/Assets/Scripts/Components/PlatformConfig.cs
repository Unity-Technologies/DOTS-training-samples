using Unity.Collections;
using Unity.Entities;

public struct PlatformConfig : IComponentData
{
    public Entity PlatformPrefab;
    // public NativeArray<Platform> Platforms;
    // public NativeArray<StationId> StationIds;
}