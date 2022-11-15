using Unity.Entities;

struct Config : IComponentData
{
    public Entity AntPrefab;
    public Entity WallPrefab; 
    public int Amount;
    
    // wall variables
    public int WallRingCount; // obstacle ring count 
    public float WallPercentage; // obstacles per ring 
    public float WallRadius; // obstacle radius 
    public int MapSize; // mapSize 
}
