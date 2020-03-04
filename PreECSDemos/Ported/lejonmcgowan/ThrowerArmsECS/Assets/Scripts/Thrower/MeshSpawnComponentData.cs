using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct MeshSpawnComponentData : IComponentData
{
    public  Entity prefab;
    public int numToSpawn;
    public Translation position;
    public float3 right;
}