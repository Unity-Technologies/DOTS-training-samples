using Unity.Entities;

public class TrainCount : IComponentData
{
    public int Value;
        
    public static implicit operator int(TrainCount v) => v.Value;
    public static implicit operator TrainCount(int v) => new TrainCount { Value = v };
}
