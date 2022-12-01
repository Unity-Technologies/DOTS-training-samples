using Unity.Entities;

struct PlatformInfo : IComponentData
{
    public int CurrentStation;
    public int DestinationStation;
    public int DestinationPlatform;
    public int CurrentPlatform;
}
