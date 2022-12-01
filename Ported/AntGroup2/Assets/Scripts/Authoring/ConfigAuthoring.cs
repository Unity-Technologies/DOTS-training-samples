using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

public class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject WallPrefab;
    public UnityEngine.GameObject AntPrefab;
    public UnityEngine.GameObject FoodPrefab;
    public UnityEngine.GameObject ColonyPrefab;
    public int TotalAmountOfAnts = 5;
    public int PlaySize = 10;
    public int AmountOfWalls = 3;
    public float TimeScale = 1.0f;
    public float RandomSteeringAmount = 0.14f;
    
    public float MoveTargetWeight = 1.0f;
    public float MoveRandomWeight = 0.8f;
    public float MoveWallWeight = 1.0f;
    public float MovePheromoneWeight = 0.3f;
    
    public int PheromoneSampleDistPixels = 5;
    public int PheromoneSampleStepAngle = 15;
    public int PheromoneSampleStepCount = 2;
    public int PheromoneSpawnDistPixels = 5;
    public float PheromoneSpawnRateSec = 0.1f;
    public float PheromoneDecayRateSec = 0.1f;

}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            AntPrefab = GetEntity(authoring.AntPrefab),
            TotalAmountOfAnts = authoring.TotalAmountOfAnts,
            WallPrefab = GetEntity(authoring.WallPrefab),
            PlaySize = authoring.PlaySize,
            AmountOfWalls = authoring.AmountOfWalls,
            FoodPrefab = GetEntity(authoring.FoodPrefab),
            ColonyPrefab = GetEntity(authoring.ColonyPrefab),
            TimeScale = authoring.TimeScale,
            RandomSteeringAmount = authoring.RandomSteeringAmount,
            
            MoveTargetWeight = authoring.MoveTargetWeight,
            MoveRandomWeight = authoring.MoveRandomWeight,
            MoveWallWeight = authoring.MoveWallWeight,
            MovePheromoneWeight = authoring.MovePheromoneWeight,
            
            PheromoneSampleDistPixels = authoring.PheromoneSampleDistPixels,
            PheromoneSampleStepAngle = authoring.PheromoneSampleStepAngle,
            PheromoneSampleStepCount = authoring.PheromoneSampleStepCount,
            PheromoneSpawnDistPixels = authoring.PheromoneSpawnDistPixels,
            PheromoneSpawnRateSec = authoring.PheromoneSpawnRateSec,
            PheromoneDecayRateSec = authoring.PheromoneDecayRateSec
        });
    }
}

