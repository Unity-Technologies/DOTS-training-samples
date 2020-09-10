using Unity.Entities;

[GenerateAuthoringComponent]
public struct UserInput : IComponentData
{
    public Entity ResourcePrefab;
}