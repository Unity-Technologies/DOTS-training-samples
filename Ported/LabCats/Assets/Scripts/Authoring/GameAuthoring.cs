using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    private float CellSize => 1.0f;
    [Range(10, 60)]public int NumberColumns;
    [Range(10, 60)]public int NumberRows;
    public GameObject LightCellPrefab;
    public GameObject DarkCellPrefab;
    public GameObject CursorPrefab;
    public GameObject PlayerCursorPrefab;
    public GameObject ArrowPrefab;
    public GameObject MousePrefab;
    public GameObject CatPrefab;
    public GameObject WallPrefab;
    public GameObject GoalPrefab;

    public int MaxCats = 3;
    public float CatSpawnFrequency = 5f;
    public float MouseSpawnFrequency = 0.25f;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(LightCellPrefab);
        referencedPrefabs.Add(DarkCellPrefab);
        referencedPrefabs.Add(CursorPrefab);
        referencedPrefabs.Add(PlayerCursorPrefab);
        referencedPrefabs.Add(ArrowPrefab);
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(CatPrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(GoalPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        var random = new Unity.Mathematics.Random(1234);
        //goals shouldnt be random, they should be computed from the board dimension to be fair
        dstManager.AddComponentData(entity,
            new BoardDefinition()
            {
                CellSize = CellSize,
                NumberColumns = NumberColumns,
                NumberRows = NumberRows,
                GoalPlayer1 = new GridPosition(){X = 2/*random.NextInt(0, NumberColumns)*/, Y=8/*random.NextInt(0, NumberRows)*/},
                GoalPlayer2 = new GridPosition(){X = random.NextInt(0, NumberColumns), Y=random.NextInt(0, NumberRows)},
                GoalPlayer3 = new GridPosition(){X = random.NextInt(0, NumberColumns), Y=random.NextInt(0, NumberRows)},
                GoalPlayer4 = new GridPosition(){X = random.NextInt(0, NumberColumns), Y=random.NextInt(0, NumberRows)},
            });
        var gridCellBuffer = dstManager.AddBuffer<GridCellContent>(entity);
        gridCellBuffer.Capacity = NumberColumns * NumberRows;
        dstManager.AddComponent<GameData>(entity);

        dstManager.AddComponentData(entity, new BoardPrefab()
        {
            LightCellPrefab = conversionSystem.GetPrimaryEntity(LightCellPrefab),
            DarkCellPrefab = conversionSystem.GetPrimaryEntity(DarkCellPrefab),
            CursorPrefab = conversionSystem.GetPrimaryEntity(CursorPrefab),
            PlayerCursorPrefab = conversionSystem.GetPrimaryEntity(PlayerCursorPrefab),
            ArrowPrefab = conversionSystem.GetPrimaryEntity(ArrowPrefab),
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            GoalPrefab = conversionSystem.GetPrimaryEntity(GoalPrefab),
        });

        dstManager.AddComponentData(entity, new DynamicSpawnerDefinition()
        {
            MousePrefab = conversionSystem.GetPrimaryEntity(MousePrefab),
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab),
            CatFrequency = CatSpawnFrequency,
            MouseFrequency = MouseSpawnFrequency,
            MaxCats = MaxCats
        });
    }
}
