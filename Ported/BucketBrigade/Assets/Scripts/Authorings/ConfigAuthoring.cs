using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameObject bucketFetcherPrefab;
    public GameObject bucketPasserPrefab;
    public GameObject omniWorkerPrefab;
    public GameObject bucketPrefab;
    public GameObject flameCellPrefab;
    public GameObject waterCellPrefab;

    public Color defaultTemperatureColour;
    public Color lowTemperatureColour;
    public Color highTemperatureColour;
    
    public Color emptyBucketColour;
    public Color fullBucketColour;

    public int numberOfTeams;
    public int bucketPassersPerTeam;
    public int bucketFetchersPerTeam;
    public int omniWorkersCount;
    public int gridSize = 1000;
    public int fireCellCount;
    public float heatTransferRate;
    // todo add fire respawn rate
    public int waterCellCount;
    public int bucketCount;

    class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config()
            {
                bucketFetcherPrefab = GetEntity(authoring.bucketFetcherPrefab),
                bucketPasserPrefab = GetEntity(authoring.bucketPasserPrefab),
                omniWorkerPrefab = GetEntity(authoring.omniWorkerPrefab),
                bucketPrefab = GetEntity(authoring.bucketPrefab),
                flameCellPrefab = GetEntity(authoring.flameCellPrefab),
                waterCellPrefab = GetEntity(authoring.waterCellPrefab),
                
                defaultTemperatureColour = authoring.defaultTemperatureColour,
                lowTemperatureColour = authoring.lowTemperatureColour,
                highTemperatureColour = authoring.highTemperatureColour,
                
                emptyBucketColour = authoring.emptyBucketColour,
                fullBucketColour = authoring.fullBucketColour,
                
                numberOfTeams = authoring.numberOfTeams,
                bucketPassersPerTeam = authoring.bucketPassersPerTeam,
                bucketFetchersPerTeam = authoring.bucketFetchersPerTeam,
                omniWorkersCount = authoring.omniWorkersCount,
                gridSize = authoring.gridSize,
                fireCellCount = authoring.fireCellCount,
                heatTransferRate = authoring.heatTransferRate,
                waterCellCount = authoring.waterCellCount,
                bucketCount = authoring.bucketCount
            });
        }
    }
}
