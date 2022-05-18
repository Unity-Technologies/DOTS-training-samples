using Unity.Entities;

struct Config : IComponentData
{
    public Entity FetcherPrefab;
    
    public Entity WorkerEmptyPrefab;
    public Entity WorkerFullPrefab;

    public Entity TilePrefab;

    public int FetcherCount;
    
    public int WorkerEmptyCount;
    public int WorkerFullCount;

    public int GridSize;
}