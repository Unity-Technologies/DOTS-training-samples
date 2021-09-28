using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct AntSpawner : IComponentData
{
    public int AntsToSpawn;
    public Entity Ant;
    public float SpawnAreaSize;
}