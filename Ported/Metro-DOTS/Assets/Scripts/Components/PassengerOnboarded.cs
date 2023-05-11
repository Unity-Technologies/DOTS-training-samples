using Unity.Entities;


public struct PassengerOnboarded : IComponentData, IEnableableComponent
{
    public int StationTrackPointIndex;
}
