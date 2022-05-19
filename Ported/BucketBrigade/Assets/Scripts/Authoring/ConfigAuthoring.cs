using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject FetcherPrefab;

    public UnityEngine.GameObject WorkerEmptyPrefab;
    public UnityEngine.GameObject WorkerFullPrefab;

    public int NbOfTeams;
    
    // Per team
    public int FetcherPerTeamCount;
    public int WorkerEmptyPerTeamCount;
    public int WorkerFullPerTeamCount;

    public int BucketCount;
    public UnityEngine.GameObject BucketPrefab;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            FetcherPrefab = GetEntity(authoring.FetcherPrefab),
            WorkerEmptyPrefab = GetEntity(authoring.WorkerEmptyPrefab),
            WorkerFullPrefab = GetEntity(authoring.WorkerFullPrefab),
            NbOfTeams = authoring.NbOfTeams,
            FetcherPerTeamCount = authoring.FetcherPerTeamCount,
            WorkerEmptyPerTeamCount = authoring.WorkerEmptyPerTeamCount,
            WorkerFullPerTeamCount = authoring.WorkerFullPerTeamCount,
            bucketCount = authoring.BucketCount,
            bucketPrefab = GetEntity(authoring.BucketPrefab)
        });
    }
}