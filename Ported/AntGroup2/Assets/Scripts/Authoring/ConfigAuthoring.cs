using Unity.Entities;
using Unity.Mathematics;

public class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject AntPrefab;
    public int TotalAmountOfAnts = 5;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            AntPrefab = GetEntity(authoring.AntPrefab),
            TotalAmountOfAnts = authoring.TotalAmountOfAnts
        });
    }
}
