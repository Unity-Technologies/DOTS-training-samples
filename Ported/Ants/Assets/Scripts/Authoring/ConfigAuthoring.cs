using Unity.Entities;
public class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject AntPrefab;
    public int Amount =1000;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            AntPrefab = GetEntity(authoring.AntPrefab),
            Amount = authoring.Amount
        });
    }
}