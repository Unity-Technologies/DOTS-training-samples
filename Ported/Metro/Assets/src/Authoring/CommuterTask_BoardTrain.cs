using Unity.Entities;

[GenerateAuthoringComponent]
public struct CommuterTask_BoardTrain : IComponentData
{
    public Entity Carriage;
    public int SeatIndex;
}
