using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TileSpawner : IComponentData
{
    public int XSize;
    public int YSize;
    public float Scale;
    public Entity Prefab;
}
