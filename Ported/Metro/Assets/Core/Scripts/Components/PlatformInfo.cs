using Unity.Entities;

struct PlatformInfo : IComponentData
{
    public int Destination;
    public int Current;
}
