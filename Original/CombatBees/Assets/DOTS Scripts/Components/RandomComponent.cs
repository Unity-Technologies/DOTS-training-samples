using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RandomComponent: IComponentData
{
    public Random Value;
}