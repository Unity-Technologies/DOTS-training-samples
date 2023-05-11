using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridAuthoring : MonoBehaviour
{
    [Header("General grid")]
    public int GridSize;
    public float MinGridY;
    public float GridCellSize;

    [Header("Bot settings")]
    public int NumOmnibot;
    
    [Header("Team Bot settings")]
    public int NumTeams;
    public int NumTeambotPassTowardsWater;
    public int NumTeambotPassTowardsFire;

    [Header("Fire settings")]
    public int NumStartingFires;
    public float FireSpreadValue;
    public float FireGrowthRate;
    public Color StartingGridColor;
    public Color FullBurningGridColor;

    [Header("Prefabs")]
    public GameObject BotPrefab;
    public GameObject FirePrefab;
    public GameObject WaterPrefab;
    public GameObject OmnibotPrefab;
    public GameObject TeambotPrefab;
    public Transform originGrid;

    class Baker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            // Each authoring field corresponds to a component field of the same name.
            AddComponent(entity, new Grid
            {
                GridSize = authoring.GridSize,
                NumStartingFires = authoring.NumStartingFires,
                MinGridY = authoring.MinGridY,
                BotPrefab = GetEntity(authoring.BotPrefab, TransformUsageFlags.Dynamic),
                FirePrefab = GetEntity(authoring.FirePrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                WaterPrefab = GetEntity(authoring.WaterPrefab,TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                OmnibotPrefab = GetEntity(authoring.OmnibotPrefab, TransformUsageFlags.Dynamic),
                GridOrigin = new float3(authoring.originGrid.position),
                NumOmnibot = authoring.NumOmnibot,
                FireSpreadValue = authoring.FireSpreadValue,
                FireGrowthRate = authoring.FireGrowthRate,
                StartingGridColor = new float4(authoring.StartingGridColor.r, authoring.StartingGridColor.g, authoring.StartingGridColor.b, authoring.StartingGridColor.a),
                FullBurningGridColor = new float4(authoring.FullBurningGridColor.r, authoring.FullBurningGridColor.g, authoring.FullBurningGridColor.b, authoring.FullBurningGridColor.a),
                
                // Teambot
                TeambotPrefab = GetEntity(authoring.TeambotPrefab, TransformUsageFlags.Dynamic),
                NumTeams = authoring.NumTeams,
                NumTeambotPassTowardsWater = authoring.NumTeambotPassTowardsWater,
                NumTeambotPassTowardsFire = authoring.NumTeambotPassTowardsFire,
            });
        }
    }
}

public struct Grid : IComponentData
{
    public int GridSize;
    public float MinGridY;
    public float GridCellSize;
    public float PlayerOffset;
    public float PlayerSpeed;
    public int NumStartingFires;
    public Entity BotPrefab;
    public Entity FirePrefab;
    public Entity WaterPrefab;
    public float3 GridOrigin;
    public Entity OmnibotPrefab;
    public int NumOmnibot;
    public float FireSpreadValue;
    public float FireGrowthRate;
    public float4 StartingGridColor;
    public float4 FullBurningGridColor;
    
    // Teambot
    public Entity TeambotPrefab;
    public int NumTeams;
    public int NumTeambotPassTowardsWater;
    public int NumTeambotPassTowardsFire;
}
