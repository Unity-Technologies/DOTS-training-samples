using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings
[GenerateAuthoringComponent]
public struct FireGridSpawner : IComponentData
{
    public Entity FirePrefab;
    public int StartingFireCount;
}
