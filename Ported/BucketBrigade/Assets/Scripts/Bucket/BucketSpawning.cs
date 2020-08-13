using Unity.Entities;

// Helper component to correctly convert a GameObject prefab to Entity prefab 
[GenerateAuthoringComponent]
public struct BucketSpawning : IComponentData
{
    public Entity Prefab;
}