using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Range(30, 50)]public float CellSize;
    [Range(20, 60)]public int NumberColumns;
    [Range(20, 60)]public int NumberRows;
    public GameObject LightCellPrefab;
    public GameObject DarkCellPrefab;
    public GameObject CursorPrefab;
    public GameObject ArrowPrefab;
    public GameObject MousePrefab;
    public GameObject CatPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(LightCellPrefab);
        referencedPrefabs.Add(DarkCellPrefab);
        referencedPrefabs.Add(CursorPrefab);
        referencedPrefabs.Add(ArrowPrefab);
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(CatPrefab);
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
            ArrowPrefab = conversionSystem.GetPrimaryEntity(ArrowPrefab),
            MousePrefab = conversionSystem.GetPrimaryEntity(MousePrefab),
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab)
        });







/*
        var playerReferenceBuffer = dstManager.AddBuffer<PlayerReference>(entity);
        playerReferenceBuffer.Capacity = 4;
        for (int i = 0; i < 4; ++i)
        {
            Entity playerEntity = dstManager.CreateEntity();
            dstManager.AddComponentData(playerEntity, new PlayerIndex() { Value = i });
            dstManager.AddComponent<Score>(playerEntity);
            dstManager.AddBuffer<ArrowReference>(playerEntity);
//            playerReferenceBuffer.Add(new PlayerReference() { Player = playerEntity });
            if (i != 0)
                dstManager.AddComponent<AITargetCell>(playerEntity);
        }

        dstManager.AddComponentData(entity, new BoardDefinition() { CellSize = CellSize, NumberColumns = NumberColumns, NumberRows = NumberRows });
        var gridCellBuffer = dstManager.AddBuffer<GridCellContent>(entity);
        gridCellBuffer.Capacity = NumberColumns * NumberRows;
        dstManager.AddComponent<GameData>(entity);*/
    }
}
