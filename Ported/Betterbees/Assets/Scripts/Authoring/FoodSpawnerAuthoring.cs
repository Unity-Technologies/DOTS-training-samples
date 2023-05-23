using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;


public class FoodSpawnerAuthoring : MonoBehaviour
{
    public GameObject foodPrefab;
    public int initialSpawnAmount;
}

public class FoodSpawnerBaker : Baker<FoodSpawnerAuthoring>
{
    public override void Bake(FoodSpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new FoodSpawnerComponent
        {
            initialSpawnAmount = authoring.initialSpawnAmount,
            foodPrefab = GetEntity(authoring.foodPrefab, TransformUsageFlags.Dynamic)
        });

        AddComponent(entity, new LocalTransform
        {
            Position = authoring.transform.position,
            Rotation = authoring.transform.rotation,
            Scale = authoring.transform.localScale.magnitude
        });
    }
}