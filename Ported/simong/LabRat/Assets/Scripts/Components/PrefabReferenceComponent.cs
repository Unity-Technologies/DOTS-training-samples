using Unity.Entities;

[GenerateAuthoringComponent]
public struct PrefabReferenceComponent : IComponentData
{
    public Entity ArrowPrefab;
}
