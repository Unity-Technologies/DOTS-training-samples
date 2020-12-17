using System;
using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

public class SettingsAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject TilePrefab;
    public GameObject RockPrefab;
    public GameObject SiloPrefab;
    public GameObject FarmerPrefab;
    public GameObject DronePrefab;
    
    public int2   GridSize;
    public float2 TileSize;
    
    public int RockSpawnAttempts;
    public int StoreSpawnCount;
    public int InitialFarmersCount;
    
    public float3 CameraOffset;
    public float  CameraDamping;

    public void OnEnable()
    {
        OnValidate();
    }

    public void OnValidate()
    {
        InitialFarmersCount = Mathf.Max(InitialFarmersCount, 1);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(TilePrefab);
        referencedPrefabs.Add(RockPrefab);
        referencedPrefabs.Add(SiloPrefab);
        referencedPrefabs.Add(FarmerPrefab);
        referencedPrefabs.Add(DronePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // Settings (Destroyed After Initialize)
        dstManager.AddComponentData(entity, new InitializationSettings
        {
            TilePrefab   = conversionSystem.GetPrimaryEntity(TilePrefab),
            RockPrefab   = conversionSystem.GetPrimaryEntity(RockPrefab),
            SiloPrefab   = conversionSystem.GetPrimaryEntity(SiloPrefab),
            InitialFarmersCount = InitialFarmersCount
        });

        // Common Settings (Available at Runtime)
        var commonSettingsEntity = conversionSystem.CreateAdditionalEntity(this);
        dstManager.AddComponentData(commonSettingsEntity, new CommonSettings
        {
            FarmerPrefab = conversionSystem.GetPrimaryEntity(FarmerPrefab),
            DronePrefab  = conversionSystem.GetPrimaryEntity(DronePrefab),
            
            GridSize          = GridSize,
            TileSize          = TileSize,
            RockSpawnAttempts = RockSpawnAttempts,
            StoreSpawnCount   = StoreSpawnCount,
            
            CameraOffset      = CameraOffset,
            CameraDamping     = CameraDamping
        });
        
        // Common Data (Available at Runtime)
        var dataEntity = conversionSystem.CreateAdditionalEntity(this);
        
        dstManager.AddComponent<CommonData>(dataEntity);

        var tileBuffer = dstManager.AddBuffer<TileState>(dataEntity);
        tileBuffer.Length = GridSize.x * GridSize.y;
    }
}