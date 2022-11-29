using Unity.Collections;
using Unity.Entities;
using UnityEngine;

class HiveAuthoring : MonoBehaviour
{
    public Color beeColor;
}

class HiveBaker : Baker<HiveAuthoring>
{
    public override void Bake(HiveAuthoring authoring)
    {
        var bounds = authoring.GetComponent<Renderer>().bounds;
        AddComponent(new Hive
        {
            color = (Vector4)authoring.beeColor,
            boundsPosition = bounds.center,
            boundsExtents = bounds.extents,
        }) ;

        AddBuffer<EnemyBees>();
        AddBuffer<AvailableResources>();
    }
}