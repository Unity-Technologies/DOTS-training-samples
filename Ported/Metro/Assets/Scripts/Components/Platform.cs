using Unity.Collections;
using Unity.Entities;

public struct Platform : IComponentData
{
    public Entity Train;
    public NativeArray<Entity> PlatformQueues;
    public Walkway LeftWalkway;
    public Walkway RightWalkway;
}