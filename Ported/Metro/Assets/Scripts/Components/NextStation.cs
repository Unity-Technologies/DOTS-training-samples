using Unity.Entities;

[GenerateAuthoringComponent]
public struct NextStation : IComponentData
{
    public int stationIndex;
}
