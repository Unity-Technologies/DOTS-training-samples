using Unity.Entities;
using Unity.Mathematics;

public struct TargetSpawnerComponentData: IComponentData
{
    public float  velocityX;
    public float2 yRange;
    //where the "kill planes" will be for the projectile while waiting to be picked up
    public float2 xRange;
    public float spawnZ;
    public float spawnFrequency;
    public int initSpawn;
    public Entity prefab;
    public float spawnRemaining;
    public float reach;
}