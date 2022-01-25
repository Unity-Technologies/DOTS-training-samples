using Unity.Entities;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity YellowBeePrefab;
    public Entity BlueBeePrefab;
    public Entity BloodPrefab;
    public Entity BeeBitsPrefab;
    public Entity ResourcePrefab;
}