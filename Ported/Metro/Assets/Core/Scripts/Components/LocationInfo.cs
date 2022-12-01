using Unity.Entities;

struct LocationInfo : IComponentData
{
    public int CurrentStation;
    public int DestinationStation;
    public int DestinationPlatform;
    public int CurrentPlatform;
}
