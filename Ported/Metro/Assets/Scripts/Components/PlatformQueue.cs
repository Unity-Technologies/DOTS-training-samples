using Unity.Collections;
using Unity.Entities;

public struct PlatformQueue : IComponentData
{
    public NativeList<Entity> Passengers; // TODO - Are we still going with this approach?
    //public int Count;
}