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
    
    public Color bucketFetcherColour;
    public Color bucketPasserColour;
    public Color omniworkerColour;
    public Color emptyBucketColour;
    public Color fullBucketColour;

    public Color defaultTemperatureColour;
    public Color lowTemperatureColour;
    public Color highTemperatureColour;
    
    public Color waterCellColour;

    public int numberOfTeams;
    public int bucketPassersPerTeam;
    public int bucketFetchersPerTeam;
    public int omniWorkersCount;
    public float workerSpeed;
    public int gridSize = 100;
    public float baseHeatIncreaseRate;
    public float heatTransferRate;
    // todo add fire respawn rate
    public int waterCellCount;
    public byte maxWaterCellWaterAmount;
    public int bucketCount;
    public byte maxBucketAmount;

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
                
                bucketFetcherColour = authoring.bucketFetcherColour,
                bucketPasserColour = authoring.bucketPasserColour,
                omniworkerColour = authoring.omniworkerColour,
                emptyBucketColour = authoring.emptyBucketColour,
                fullBucketColour = authoring.fullBucketColour,
                
                defaultTemperatureColour = authoring.defaultTemperatureColour,
                lowTemperatureColour = authoring.lowTemperatureColour,
                highTemperatureColour = authoring.highTemperatureColour,

                waterCellColour = authoring.waterCellColour,

                numberOfTeams = authoring.numberOfTeams,
                bucketPassersPerTeam = authoring.bucketPassersPerTeam,
                bucketFetchersPerTeam = authoring.bucketFetchersPerTeam,
                omniWorkersCount = authoring.omniWorkersCount,
                workerSpeed = authoring.workerSpeed,
                gridSize = authoring.gridSize,
                baseHeatIncreaseRate = authoring.baseHeatIncreaseRate,
                heatTransferRate = authoring.heatTransferRate,
                waterCellCount = authoring.waterCellCount,
                maxWaterCellWaterAmount = authoring.maxWaterCellWaterAmount,
                bucketCount = authoring.bucketCount,
                maxBucketAmount = authoring.maxBucketAmount
            });
        }
    }
}
