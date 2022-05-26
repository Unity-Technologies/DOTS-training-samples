using Unity.Entities;
using Unity.Mathematics;
struct GameConfig : IComponentData
{
    public Entity FarmerPrefab;
    public int InitialFarmerCount;

    public Entity DronePrefab;
 
    public int2 MapSize;

    public Entity GroundTileNormalPrefab;
    public Entity GroundTileTilledPrefab;
    public Entity GroundTileUnpassablePrefab;

    public Entity PlantPrefab;
    
    public float PlantIncubationTime;

    public Entity SiloPrefab;

    public int WorldGenerationSeed;

    public Entity RockPrefab;
    public int InitialRockAttempts;
    public float MinRockSize;
    public float MaxRockSize;
    public int RockHealthPerUnitArea;
    public float MinRockDepth;
    public float MaxRockDepth;

    public int PathfindingAcquisitionRange;
    public int RockSmashActionRange;
    public float RockDamagePerHit;

    public float FarmerAttackCooldown;
    public float FarmerMoveSpeed;

    public int CostToSpawnFarmer;
    public int CostToSpawnDrone;
    public int MoneyPerPlant;
}