using Unity.Entities;
using Unity.Transforms;

public struct MeshSpawnComponentData : IComponentData
{
    public  Entity prefab;
    public int numToSpawn;
    public LocalToWorld transform;
}