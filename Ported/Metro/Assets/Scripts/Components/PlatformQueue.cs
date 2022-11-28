using Unity.Collections;
using Unity.Entities;

public struct PlatformQueue : IComponentData
{
    public NativeList<Entity> Passengers;
}