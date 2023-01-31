using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public List<GameObject> Lines;
    public GameObject CarriagePrefab;
    public GameObject CommuterPrefab;
    public GameObject PlatformPrefab;
    public int CommuterCount;
    public int LineCount;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config()
            {
                Lines = authoring.Lines.ConvertAll(x => GetEntity(x)).ToNativeList(Allocator.Persistent),
                CarriagePrefab = GetEntity(authoring.CarriagePrefab),
                CommuterPrefab = GetEntity(authoring.CommuterPrefab),
                PlatformPrefab = GetEntity(authoring.PlatformPrefab),
                CommuterCount = authoring.CommuterCount,
                LineCount = authoring.CommuterCount,
            });
        }
    }
}

struct Config : IComponentData
{
    public NativeList<Entity> Lines;
    public Entity CarriagePrefab;
    public Entity CommuterPrefab;
    public Entity PlatformPrefab;
    public int CommuterCount;
    public int LineCount;
}
