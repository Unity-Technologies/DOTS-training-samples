using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class GameConfigAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject CellPrefab;

    public float MouseSpeed;
    public float CatSpeed;
    public int RoundDuration;
    public int2 BoardDimensions;
    public Color TileColor1;
    public Color TileColor2;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CellPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var gameConfig = new GameConfig()
        {
            CellPrefab = conversionSystem.GetPrimaryEntity(CellPrefab),
            MouseSpeed = MouseSpeed,
            CatSpeed = CatSpeed,
            RoundDuration = RoundDuration,
            BoardDimensions = BoardDimensions,
            TileColor1 = new float4(TileColor1.r, TileColor1.g, TileColor1.b, 1),
            TileColor2 = new float4(TileColor2.r, TileColor2.g, TileColor2.b, 1)
        };

        dstManager.AddComponentData(entity, gameConfig);
    }
}
