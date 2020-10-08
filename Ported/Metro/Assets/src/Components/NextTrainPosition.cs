using Unity.Entities;

public struct NextTrainPosition : IComponentData
{
    public float Value;
    
    public static implicit operator float(NextTrainPosition v) => v.Value;
    public static implicit operator NextTrainPosition(float v) => new NextTrainPosition { Value = v };
}
