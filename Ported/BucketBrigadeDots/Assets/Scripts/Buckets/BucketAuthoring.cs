using Unity.Entities;
using UnityEngine;

public class BucketAuthoring : MonoBehaviour
{
    public GameObject BucketPrefab;
    public int NumberOfBuckets = 2;
}

public class BucketBaker : Baker<BucketAuthoring>
{
    public override void Bake(BucketAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.WorldSpace);

        AddComponent(entity, new BucketSpawner() {
            BucketPrefab = GetEntity(authoring.BucketPrefab, TransformUsageFlags.WorldSpace),
            NumberOfBuckets = authoring.NumberOfBuckets,
        });
    }
}
