using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

class BeeConfigAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject BeePrefab;
    public GameObject FoodPrefab;
    public int BeeCount;
    public int FoodCount;
}

class BeeConfigBaker : Baker<BeeConfigAuthoring>
{
    public override void Bake(BeeConfigAuthoring authoring)
    {
        AddComponent<BeeConfig>(new BeeConfig()
        {
            bee = GetEntity(authoring.BeePrefab),
            food = GetEntity(authoring.FoodPrefab),
            beeCount = authoring.BeeCount,
            foodCount = authoring.FoodCount
        });
    }
}
