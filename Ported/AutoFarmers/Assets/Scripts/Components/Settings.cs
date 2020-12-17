using Unity.Entities;
using Unity.Mathematics;

public struct InitializationSettings : IComponentData
{
    public Entity TilePrefab;
    public Entity RockPrefab;
    public Entity SiloPrefab;
    public int    InitialFarmersCount;
}

public struct CommonSettings : IComponentData
{
    public Entity FarmerPrefab;
    public Entity DronePrefab;
    
    public int2   GridSize;
    
    public int RockSpawnAttempts;
    public int StoreSpawnCount;
    
    public float2 CameraViewAngle;
    public float  CameraViewDistance;
    public float  CameraMouseSensitivity;
}