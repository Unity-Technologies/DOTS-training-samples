using Unity.Entities;
using Unity.Mathematics;

public struct LineComponent : IComponentData
{
    public int TrainCount;
    public int CarriageCount;
    public float CarriageLength;
    public float MaxSpeed;
    public float LineLength;
}