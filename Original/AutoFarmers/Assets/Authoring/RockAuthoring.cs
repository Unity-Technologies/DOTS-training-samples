using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RockAuthoring : IComponentData
{
    public Entity rockEntity;
    public int mapX;
    public int mapY;
}