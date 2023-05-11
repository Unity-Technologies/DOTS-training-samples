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
    public int NumTeams = 1;
    public int NumTeambotPassTowardsWater = 6;
    public int NumTeambotPassTowardsFire = 6;
    public float TeambotTravelSpeed = 5;
    public float TeambotDouseRadius = 7;
    public float TeambotMaxDouseAmount = 1.8f;
    public float TeambotWaterFillDuration = 2;
    public float TeambotWaterGatherSpeed = 0.1f;

    [Header("Water")]
    public float WaterRefillSpeed = 0.05f;

    [Header("Fire settings")]
    public int NumStartingFires;
    public float FireSpreadValue;
    public float FireGrowthRate;
    public Color StartingGridColor;
    public Color FullBurningGridColor;
    public float RadiusClickFire;

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
                FirePrefab = GetEntity(authoring.FirePrefab,
                    TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                WaterPrefab = GetEntity(authoring.WaterPrefab,
                    TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale),
                OmnibotPrefab = GetEntity(authoring.OmnibotPrefab, TransformUsageFlags.Dynamic),
                GridOrigin = new float3(authoring.originGrid.position),
                NumOmnibot = authoring.NumOmnibot,
                FireSpreadValue = authoring.FireSpreadValue,
                FireGrowthRate = authoring.FireGrowthRate,
                StartingGridColor = new float4(authoring.StartingGridColor.r, authoring.StartingGridColor.g,
                    authoring.StartingGridColor.b, authoring.StartingGridColor.a),
                FullBurningGridColor = new float4(authoring.FullBurningGridColor.r, authoring.FullBurningGridColor.g,
                    authoring.FullBurningGridColor.b, authoring.FullBurningGridColor.a),

                RadiusClickFire = authoring.RadiusClickFire,
                
                // Teambot
                TeambotPrefab = GetEntity(authoring.TeambotPrefab, TransformUsageFlags.Dynamic),
                NumTeams = authoring.NumTeams,
                NumTeambotPassTowardsWater = authoring.NumTeambotPassTowardsWater,
                NumTeambotPassTowardsFire = authoring.NumTeambotPassTowardsFire,
                TeambotTravelSpeed = authoring.TeambotTravelSpeed,
                TeambotDouseRadius = authoring.TeambotDouseRadius,
                TeambotMaxDouseAmount = authoring.TeambotMaxDouseAmount,
                TeambotWaterFillDuration = authoring.TeambotWaterFillDuration,
                // TeambotWaterFillElapsedTime = authoring.TeambotWaterFillElapsedTime,
                TeambotWaterGatherSpeed = authoring.TeambotWaterGatherSpeed,
                
                // Water
                WaterRefillSpeed = authoring.WaterRefillSpeed,
            });
            AddComponent<MouseHit>(entity);
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
    public float RadiusClickFire;
    public float4 StartingGridColor;
    public float4 FullBurningGridColor;

    // Teambot
    public Entity TeambotPrefab;
    public int NumTeams;
    public int NumTeambotPassTowardsWater;
    public int NumTeambotPassTowardsFire;
    public float TeambotTravelSpeed;
    public float TeambotDouseRadius;
    public float TeambotMaxDouseAmount;
    public float TeambotWaterFillDuration;
    public float TeambotWaterGatherSpeed;
    
    // Water 
    public float WaterRefillSpeed;
}

// For mouse hit
public struct MouseHit : IComponentData {
    public float3 Value;
    public bool ChangedThisFrame;

}