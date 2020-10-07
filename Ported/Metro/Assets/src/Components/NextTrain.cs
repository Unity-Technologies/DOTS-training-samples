using Unity.Entities;

public struct NextTrain : IComponentData
{
    public Entity Value;
            
    public static implicit operator Entity(NextTrain v) => v.Value;
    public static implicit operator NextTrain(Entity v) => new NextTrain { Value = v };
}
