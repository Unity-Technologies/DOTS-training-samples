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
    public UnityGameObject CatPrefab;
    public UnityGameObject WallPrefab;
    public UnityGameObject ArrowPrefab;
    public int NumOfCats;
    public int NumOfAIPlayers;
    public float MouseSpeed = 1f;
    public float CatSpeed = .5f;
    public int RoundDuration = 30;
    public int2 BoardDimensions = new int2(8,8);
    public Color TileColor1 = Color.gray;
    public Color TileColor2 = Color.yellow;

    public float WallProbability = .1f;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CellPrefab);
        referencedPrefabs.Add(CatPrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(ArrowPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var gameConfig = new GameConfig()
        {
            CellPrefab = conversionSystem.GetPrimaryEntity(CellPrefab),
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab),
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            ArrowPrefab = conversionSystem.GetPrimaryEntity(ArrowPrefab),
            MouseSpeed = MouseSpeed,
            CatSpeed = CatSpeed,
            NumOfCats =  NumOfCats,
            NumOfAIPlayers = NumOfAIPlayers,
            RoundDuration = RoundDuration,
            BoardDimensions = BoardDimensions,
            TileColor1 = new float4(TileColor1.r, TileColor1.g, TileColor1.b, 1),
            TileColor2 = new float4(TileColor2.r, TileColor2.g, TileColor2.b, 1),
            WallProbability = WallProbability
        };

        dstManager.AddComponentData(entity, gameConfig);
    }
}
