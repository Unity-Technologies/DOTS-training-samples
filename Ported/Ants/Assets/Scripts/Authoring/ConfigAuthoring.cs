using Unity.Entities;
public class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject AntPrefab;
    public UnityEngine.GameObject WallPrefab;
    public int Amount =1000;
    public int WallRingCount = 3; // obstacle ring count 
    public float WallPercentage = 0.8f; // obstacles per ring 
    public float WallRadius = 3f; // obstacle radius 
    public int MapSize = 128; // mapSize 
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            AntPrefab = GetEntity(authoring.AntPrefab),
            WallPrefab = GetEntity(authoring.WallPrefab),
            Amount = authoring.Amount,
            WallRingCount = authoring.WallRingCount,
            WallPercentage = authoring.WallPercentage,
            WallRadius = authoring.WallRadius,
            MapSize = authoring.MapSize
        });
    }
}