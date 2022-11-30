using Unity.Collections;
using Unity.Entities;

public struct Station : IComponentData
{
    public NativeList<Platform> Platforms;
}