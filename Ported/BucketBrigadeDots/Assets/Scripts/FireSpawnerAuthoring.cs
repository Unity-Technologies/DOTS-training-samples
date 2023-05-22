using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FireSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;

    class Baker : Baker<FireSpawnerAuthoring>
    {
        public override void Bake(FireSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new FireSpawner
            {
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.WorldSpace)
            });
        }
    }
}

public struct FireSpawner : IComponentData
{
    public Entity Prefab;
}