using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

public class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject WallPrefab;
    public UnityEngine.GameObject AntPrefab;
    public int TotalAmountOfAnts = 5;
    public int PlaySize = 10;
    public int AmountOfWalls = 3;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            AntPrefab = GetEntity(authoring.AntPrefab),
            TotalAmountOfAnts = authoring.TotalAmountOfAnts,
            WallPrefab = GetEntity(authoring.WallPrefab),
            PlaySize = authoring.PlaySize,
            AmountOfWalls = authoring.AmountOfWalls
        });
    }
}

