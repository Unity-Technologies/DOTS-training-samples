using Unity.Entities;

[GenerateAuthoringComponent]
public class Spawner : IComponentData
{
    public int CubeCount;
    public Entity CubePrefab;
    public Entity BeamPrefab;
    public int TowerCount;
    public int GroundPoints;
}
