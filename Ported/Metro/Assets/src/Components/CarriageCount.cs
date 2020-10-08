using Unity.Entities;

public class CarriageCount : IComponentData
{
    public int Value;
        
    public static implicit operator int(CarriageCount v) => v.Value;
    public static implicit operator CarriageCount(int v) => new CarriageCount { Value = v };
}