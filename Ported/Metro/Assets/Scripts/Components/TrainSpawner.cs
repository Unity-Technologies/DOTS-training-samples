using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrainSpawner : IComponentData
{
    public Entity CarriagePrefab;
    public Entity RailPrefab;
    public Entity Sphere;
    public Entity PlatformPrefab;
    
    public int TrainsPerLine;
}
