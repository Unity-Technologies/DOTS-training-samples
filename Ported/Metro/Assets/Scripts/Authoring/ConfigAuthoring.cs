using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject CarriagePrefab;
    public GameObject TrainPrefab;
    public GameObject CommuterPrefab;
    public GameObject PlatformPrefab;
    public GameObject StationPrefab;
    public GameObject LinePrefab;

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
                //Lines = authoring.Lines.ConvertAll(x => GetEntity(x)).ToNativeList(Allocator.Persistent),
                CarriagePrefab = GetEntity(authoring.CarriagePrefab),
                CommuterPrefab = GetEntity(authoring.CommuterPrefab),
                PlatformPrefab = GetEntity(authoring.PlatformPrefab),
                StationPrefab = GetEntity(authoring.StationPrefab),
                TrainPrefab = GetEntity(authoring.TrainPrefab),
                LinePrefab = GetEntity(authoring.LinePrefab),
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
    public Entity CarriagePrefab;
    public Entity TrainPrefab;
    public Entity CommuterPrefab;
    public Entity PlatformPrefab;
    public Entity StationPrefab;
    public Entity LinePrefab;
    public int CommuterCount;
    public int LineCount;
    public int LineOffset;
    public int StationsPerLineCount;
    public int StationsOffset;
}
