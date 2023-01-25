using Unity.Entities;
using UnityEngine;

public struct Config : IComponentData
{
    public Entity bucketFetcherPrefab;
    public Entity bucketPasserPrefab;
    public Entity omniWorkerPrefab;
    public Entity bucketPrefab;
    public Entity flameCellPrefab;
    public Entity waterCellPrefab;

    public Color defaultTemperatureColour;
    public Color lowTemperatureColour;
    public Color highTemperatureColour;

    public Color emptyBucketColour;
    public Color fullBucketColour;

    public int numberOfTeams;
    public int bucketPassersPerTeam;
    public int bucketFetchersPerTeam;
    public int omniWorkersCount;
    // fire
    public int gridSize;
    public float heatTransferRate;
    public float baseHeatIncreaseRate;
    public int waterCellCount;
    public int bucketCount;
}
