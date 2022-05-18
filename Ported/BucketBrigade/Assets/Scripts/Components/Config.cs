using Unity.Entities;

struct Config : IComponentData
{
    public Entity FetcherPrefab;
    
    public Entity WorkerEmptyPrefab;
    public Entity WorkerFullPrefab;

    public int FetcherCount;
    
    public int WorkerEmptyCount;
    public int WorkerFullCount;

    public int bucketCount;
    public Entity bucketPrefab;
}