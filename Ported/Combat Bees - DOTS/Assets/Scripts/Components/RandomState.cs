using Unity.Entities;
using Random = Unity.Mathematics.Random;

public struct RandomState : IComponentData
{
    public Random Value;
}