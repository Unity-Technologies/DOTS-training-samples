using Unity.Entities;
using Unity.Mathematics;

public struct WallsSpawner : IComponentData
{
    public float radius;
    public float numSpheres;
    public float3 position;
    public Entity environmentPrefab;
}
public struct Walls : IComponentData
{
}