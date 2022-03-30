using Unity.Entities;

// Reads from shared components requires WithoutBurst, so both are added so both filtering and burst-access can work. They're only a byte each.
public struct TeamShared : ISharedComponentData
{
    public byte TeamId;
}

public struct Team : IComponentData
{
    public byte TeamId;
}
