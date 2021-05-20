using System;
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
    [Header("Board Generation")]
    public bool RandomSeed;
    public uint Seed = 23498573;
    public UnityGameObject CellPrefab;
    public UnityGameObject WallPrefab;
    public int2 BoardDimensions = new int2(8,8);
    public float WallProbability = .1f;
    public Color TileColor1 = Color.gray;
    public Color TileColor2 = Color.yellow;

    [Header("Game Properties")]
    public int RoundDuration = 30;
    public int NumOfAIPlayers;
    public int MaximumArrows = 3;
    public UnityGameObject ArrowPrefab;
    public UnityGameObject CursorPrefab;
    public UnityGameObject HomebasePrefab;
    public int MaxAnimalsSpawnedPerFrame = 1;
    public float TimeScale = 1;

    [Header("Mice")]
    public int NumOfMice;
    public float MouseSpawnDelay = 0.2f;
    public float MouseSpeed = 1f;
    public UnityGameObject MousePrefab;
    public bool MiceSpawnInRandomLocations = false;

    [Header("Cats")]
    public int NumOfCats;
    public float CatSpawnDelay = 1f;
    public float CatSpeed = .5f;
    public UnityGameObject CatPrefab;

    [Header("Particles")]
    public UnityGameObject ParticlePrefab;
    public float ParticleSpawnRate = 20f;
    public float ParticleLifetime = 0.5f;
    
    void Update()
    {
        Time.timeScale = TimeScale;
    }

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CellPrefab);
        referencedPrefabs.Add(CatPrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(ArrowPrefab);
        referencedPrefabs.Add(MousePrefab);
        referencedPrefabs.Add(CursorPrefab);
        referencedPrefabs.Add(HomebasePrefab);
        referencedPrefabs.Add(ParticlePrefab);

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
            BoardDimensions = BoardDimensions,
            TileColor1 = new float4(TileColor1.r, TileColor1.g, TileColor1.b, 1),
            TileColor2 = new float4(TileColor2.r, TileColor2.g, TileColor2.b, 1),
            MaximumArrows = MaximumArrows,
            WallProbability = WallProbability,
            RandomSeed = RandomSeed,
            Seed = Seed,
            MouseSpawnDelay = MouseSpawnDelay,
            CatSpawnDelay = CatSpawnDelay,
            HomebasePrefab = conversionSystem.GetPrimaryEntity(HomebasePrefab),
            MaxAnimalsSpawnedPerFrame = MaxAnimalsSpawnedPerFrame,
            MiceSpawnInRandomLocations = MiceSpawnInRandomLocations,
            ParticlePrefab = conversionSystem.GetPrimaryEntity(ParticlePrefab),
            ParticleLifetime = ParticleLifetime,
            ParticleSpawnRate = ParticleSpawnRate,
        };

        dstManager.AddComponentData(entity, gameConfig);
    }
}
