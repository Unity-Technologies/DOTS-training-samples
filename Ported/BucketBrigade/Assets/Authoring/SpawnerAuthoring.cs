using Unity.Entities;

[GenerateAuthoringComponent]
public class Spawner : IComponentData
{
    public Entity scooperPrefab;
    public Entity bucketPrefab;
}
