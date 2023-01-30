using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public List<GameObject> Lines;
    public GameObject CarriagePrefab;
    public int CommuterCount;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config()
            {
                Lines = authoring.Lines.ConvertAll(x => GetEntity(x)).ToNativeList(Allocator.Persistent),
                CarriagePrefab = GetEntity(authoring.CarriagePrefab),
                CommuterCount = authoring.CommuterCount,
            });
        }
    }
}

struct Config : IComponentData
{
    public NativeList<Entity> Lines;
    public Entity CarriagePrefab;
    public int CommuterCount;
}
