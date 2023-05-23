using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// TODO: this is really similar to FireSpawnerAuthoring. Can we deduplicate
// or templatize?
public class WaterSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;

    class Baker : Baker<WaterSpawnerAuthoring>
    {
        public override void Bake(WaterSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new WaterSpawner
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.WorldSpace),
            });
        }
    }
}

public struct WaterSpawner : IComponentData
{
    public Entity Prefab;
}