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

    public bool RandomSeed;
    public uint Seed = 23498573;

    public UnityGameObject CellPrefab;
    public UnityGameObject CatPrefab;
    public UnityGameObject WallPrefab;
    public UnityGameObject ArrowPrefab;
    public float SnapDistance = 0.2f;
    public int NumOfCats;
    public int NumOfAIPlayers;
    public UnityGameObject MousePrefab;
    public int NumOfMice;
    public float MouseSpeed = 1f;
    public float CatSpeed = .5f;
    public int RoundDuration = 30;
    public int2 BoardDimensions = new int2(8,8);
    public Color TileColor1 = Color.gray;
    public Color TileColor2 = Color.yellow;
    public UnityGameObject CursorPrefab;
    public int MaximumArrows = 3;

    public float WallProbability = .1f;
    public float MouseSpawnDelay = 0.2f;
    public float CatSpawnDelay = 1f;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CellPrefab);
        referencedPrefabs.Add(CatPrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(ArrowPrefab);
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(CursorPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var gameConfig = new GameConfig()
        {
            CellPrefab = conversionSystem.GetPrimaryEntity(CellPrefab),
            CatPrefab = conversionSystem.GetPrimaryEntity(CatPrefab),
            MousePrefab = conversionSystem.GetPrimaryEntity(MousePrefab),
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),
            ArrowPrefab = conversionSystem.GetPrimaryEntity(ArrowPrefab),
            CursorPrefab = conversionSystem.GetPrimaryEntity(CursorPrefab),
            MouseSpeed = MouseSpeed,
            CatSpeed = CatSpeed,
            NumOfCats =  NumOfCats,
            NumOfAIPlayers = NumOfAIPlayers,
            NumOfMice = NumOfMice,
            RoundDuration = RoundDuration,
            SnapDistance = SnapDistance,
            BoardDimensions = BoardDimensions,
            TileColor1 = new float4(TileColor1.r, TileColor1.g, TileColor1.b, 1),
            TileColor2 = new float4(TileColor2.r, TileColor2.g, TileColor2.b, 1),
            MaximumArrows = MaximumArrows,
            WallProbability = WallProbability,
            RandomSeed = RandomSeed,
            Seed = Seed,
            MouseSpawnDelay = MouseSpawnDelay,
            CatSpawnDelay = CatSpawnDelay
        };

        dstManager.AddComponentData(entity, gameConfig);
    }
}
