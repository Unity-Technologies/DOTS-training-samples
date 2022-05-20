using Unity.Entities;

struct Team : IComponentData
{
    public int TeamNb;
    public int FetcherIdx;
    public int WorkerIdx;
    public int NbOfFiremen;
}