using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct WaterBucketSpawner : IComponentData
{
    public Entity Prefab;
    public int Count;
}
