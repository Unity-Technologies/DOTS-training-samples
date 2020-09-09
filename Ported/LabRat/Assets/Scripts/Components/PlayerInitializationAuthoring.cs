using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerInitialization : IComponentData
{
    public Entity PlayerPrefab;
    public int PlayerCount;
}
