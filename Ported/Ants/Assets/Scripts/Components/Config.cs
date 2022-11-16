using Unity.Entities;

struct Config : IComponentData
{
    public Entity AntPrefab;
    public Entity WallPrefab;
    public Entity ColonyPrefab;
    public Entity ResourcePrefab; 
    public int Amount;
    
    // wall variables
    public int WallRingCount; // obstacle ring count 
    public float WallPercentage; // obstacles per ring 
    public float WallRadius; // obstacle radius 
    public int MapSize; // mapSize 
}
