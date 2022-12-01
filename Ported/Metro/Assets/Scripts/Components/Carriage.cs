using Unity.Entities;

public struct Carriage : IComponentData
{
    public int Index;

    public Entity Train;
    public int uniqueTrainID;
}