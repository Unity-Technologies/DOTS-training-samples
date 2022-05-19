using Unity.Entities;

struct Config : IComponentData
{
    public Entity FetcherPrefab;
    
    public Entity WorkerEmptyPrefab;
    public Entity WorkerFullPrefab;

    public int NbOfTeams;
    public int FetcherPerTeamCount;
    public int WorkerEmptyPerTeamCount;
    public int WorkerFullPerTeamCount;
    
    public int bucketCount;
    public Entity bucketPrefab;
}