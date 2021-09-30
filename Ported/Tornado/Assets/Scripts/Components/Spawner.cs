using Unity.Entities;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public int CubeCount;
    public Entity CubePrefab;
    public Entity BeamPrefab;
    public int TowerCount;
    public int GroundPoints;
    public int maxTowerHeight;
    public float groundToCoverSize;
}
