using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject WorkerEmptyPrefab;
    public UnityEngine.GameObject WorkerFullPrefab;

    public UnityEngine.GameObject TilePrefab;
    
    public int WorkerEmptyCount;
    public int WorkerFullCount;

    public int GridSize;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            WorkerEmptyPrefab = GetEntity(authoring.WorkerEmptyPrefab),
            WorkerFullPrefab = GetEntity(authoring.WorkerFullPrefab),
            TilePrefab = GetEntity(authoring.TilePrefab),
            WorkerEmptyCount = authoring.WorkerEmptyCount,
            WorkerFullCount = authoring.WorkerFullCount,
            GridSize = authoring.GridSize
        });
    }
}