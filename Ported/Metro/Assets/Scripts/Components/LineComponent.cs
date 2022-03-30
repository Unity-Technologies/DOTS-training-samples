using Unity.Entities;
public struct LineComponent : IComponentData
{
    public int TrainCount;
    public int CarriageCount;
    public float MaxSpeed;
}