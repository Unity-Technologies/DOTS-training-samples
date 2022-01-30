using Unity.Entities;

[GenerateAuthoringComponent]
public struct CurrentRoute : IComponentData
{
    public int targetStationIndex;
    public float routeStartLocation;
    public float routeEndLocation;  // Always > routeStartLocation, goes over PathLengh for ease of computations
    public TrainState state;
}
