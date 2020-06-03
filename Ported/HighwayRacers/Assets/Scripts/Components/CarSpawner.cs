using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarSpawner : IComponentData
{
    public Entity CarPrefab;
}