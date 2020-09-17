using Unity.Entities;

[GenerateAuthoringComponent]
public struct WallSettings : IComponentData
{
    public Entity wallPrefab;
}
