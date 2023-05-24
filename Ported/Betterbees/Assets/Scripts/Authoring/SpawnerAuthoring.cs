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

        switch(authoring.hiveTag)
        {
            case HiveTag.HiveYellow:
                AddComponent(entity, new HiveYellow()); break;
            case HiveTag.HiveBlue:
                AddComponent(entity, new HiveBlue()); break;
            case HiveTag.HiveOrange:
                AddComponent(entity, new HiveOrange()); break;
        }
    }
}
