using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnPlayingFieldConfig : IComponentData
{
    public Entity PlayingFieldPrefab;
}
