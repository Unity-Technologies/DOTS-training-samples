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
    public GameObject StationPrefab;

    public int CommuterCount;
    public int LineCount;
    public int LineOffset;
    public int StationsPerLineCount;
    public int StationsOffset;

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
                StationPrefab = GetEntity(authoring.StationPrefab),
                CommuterCount = authoring.CommuterCount,
                LineCount = authoring.LineCount,
                LineOffset = authoring.LineOffset,
                StationsPerLineCount = authoring.StationsPerLineCount,
                StationsOffset = authoring.StationsOffset
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
    public Entity StationPrefab;
    public int CommuterCount;
    public int LineCount;
    public int LineOffset;
    public int StationsPerLineCount;
    public int StationsOffset;
}
