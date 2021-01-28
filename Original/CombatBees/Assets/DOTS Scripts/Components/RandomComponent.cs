using Unity.Entities;
using Unity.Mathematics;

public struct RandomComponent: IComponentData
{
    public Random Value;
}