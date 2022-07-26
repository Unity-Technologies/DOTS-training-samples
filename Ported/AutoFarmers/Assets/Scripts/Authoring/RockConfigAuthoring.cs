
using Unity.Entities;

class RockConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RockPrefab;

}

class RockConfigBaker : Baker<RockConfigAuthoring>
{
    public override void Bake(RockConfigAuthoring authoring)
    {
        AddComponent(new RockConfig
        {
            RockPrefab = GetEntity(authoring.RockPrefab)
        });
    }
}