using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Range(1, 2)]public float CellSize;
    [Range(20, 60)]public int NumberColumns;
    [Range(20, 60)]public int NumberRows;
    public GameObject LightCellPrefab;
    public GameObject DarkCellPrefab;
    public GameObject CursorPrefab;
    public GameObject PlayerCursorPrefab;
    public GameObject ArrowPrefab;
    public GameObject MousePrefab;
    public GameObject CatPrefab;
    public GameObject WallPrefab;
    public GameObject MouseSpawnerPrefab;
    public GameObject CatSpawnerPrefab;
    public GameObject GoalPrefab;

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
        referencedPrefabs.Add(MouseSpawnerPrefab);
        referencedPrefabs.Add(CatSpawnerPrefab);
        referencedPrefabs.Add(GoalPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BoardDefinition() { CellSize = CellSize, NumberColumns = NumberColumns, NumberRows = NumberRows });
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
            MousePrefab = conversionSystem.GetPrimaryEntity(MousePrefab),
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab),
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            MouseSpawnerPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            CatSpawnerPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            GoalPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
        });
    }
}
