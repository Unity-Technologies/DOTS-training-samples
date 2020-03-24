using Unity.Entities;
using Unity.Mathematics;

public struct  ProjectileSpawnerComponentData: IComponentData
{
    public float  velocityX;
    public float2 radiusRange;
    //where the "kill planes" will be for the projectile while waiting to be picked up
    public float2 xRange;
    public float spawnZ;
    public float spawnFrequency;
    public float spawnRamaining;
    public int initSpawn;
    public Entity prefab;
    public int numBuckets;
}
