using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum HiveTag
{
    HiveYellow,
    HiveBlue,
    HiveOrange,
    HiveCount
}

public static class HiveTagExtensions
{
    public static System.Type ToComponentType(this HiveTag hiveTag)
    {
        switch (hiveTag)
        {
            case HiveTag.HiveYellow:
                return typeof(HiveYellow);
            case HiveTag.HiveBlue:
                return typeof(HiveBlue);
            case HiveTag.HiveOrange:
            default:
                return typeof(HiveOrange);
        }
    }
}

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int initialSpawnAmount;
    public HiveTag hiveTag;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BeeSpawnerComponent
            {
                initialSpawnAmount = authoring.initialSpawnAmount,
                beePrefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                minBounds = authoring.transform.position - 0.5f * authoring.transform.localScale,
                maxBounds = authoring.transform.position + 0.5f * authoring.transform.localScale,
                hiveTag = authoring.hiveTag
            });
            
        AddComponent(entity, new LocalTransform
        {
            Position = authoring.transform.position,
            Rotation = authoring.transform.rotation,
            Scale = authoring.transform.localScale.magnitude
        });

        AddComponent(entity, new(authoring.hiveTag.ToComponentType()));
    }
}
