using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Linq;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum MapDebugOptions
{
	Default,
	Empty,
	Tilled,
	// Plants,
}

public class Farm : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Header("DOTS settings")]
    public GameObject defaultGround;
    public GameObject tilledGround;
    public GameObject rockPrefab;
    public GameObject farmerPrefab;
    public GameObject dronePrefab;
    public GameObject storePrefab;
    public int initialMoneyForFarmers = 15;
    public int initialMoneyForDrones = 0;
    public int numberOfStores = 5;


    public MapDebugOptions debugOptions = MapDebugOptions.Default;

    [Space(30)]

    public Vector2Int mapSize;
    public static Farm instance;
    public static int seedOffset;

    void AwakeFromConvert()
    {
        instance = this;
        seedOffset = Random.Range(int.MinValue,int.MaxValue);

    }

    public void Convert(Entity entity,EntityManager dstManager,GameObjectConversionSystem conversionSystem)
    {
        AwakeFromConvert();

		var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var root = ref blobBuilder.ConstructRoot<GroundDataRegistry>();
		var defaultTile = conversionSystem.GetPrimaryEntity(defaultGround);
		var tilledTile = conversionSystem.GetPrimaryEntity(tilledGround);

		var blobRef = blobBuilder.CreateBlobAssetReference<GroundDataRegistry>(Allocator.Persistent);
		dstManager.AddComponentData<GroundData>(entity, new GroundData{
			registry = blobRef,
			defaultGroundEntity = defaultTile,
			tilledGroundEntity = tilledTile,
			debugOptions = debugOptions,
			fieldSizeX = mapSize.x,
			fieldSizeY = mapSize.y,
		});

        var originalRock = conversionSystem.GetPrimaryEntity(rockPrefab);
        dstManager.AddComponentData<RockAuthoring>(entity,new RockAuthoring
        {
            rockEntity = originalRock,
            mapX = mapSize.x,
            mapY = mapSize.y
        });

        var originalFarmer = conversionSystem.GetPrimaryEntity(farmerPrefab);
        dstManager.AddComponentData<FarmerData_Spawner>(entity,new FarmerData_Spawner
        {
            prefab = originalFarmer
        });

        var originalDrone = conversionSystem.GetPrimaryEntity(dronePrefab);
        dstManager.AddComponentData<DroneData_Spawner>(entity,new DroneData_Spawner
        {
            prefab = originalDrone
        });

        dstManager.AddComponentData<EconomyData>(entity,new EconomyData
        {
            moneyForFarmers = initialMoneyForFarmers,
            moneyForDrones = initialMoneyForDrones
        });

        var originalStore = conversionSystem.GetPrimaryEntity(storePrefab);
        dstManager.AddComponentData<StoreData_Spawner>(entity,new StoreData_Spawner
        {
            prefab = originalStore,
            count = numberOfStores,
            mapX = mapSize.x,
            mapY = mapSize.y
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(defaultGround);
        referencedPrefabs.Add(tilledGround);
        referencedPrefabs.Add(rockPrefab);
        referencedPrefabs.Add(farmerPrefab);
        referencedPrefabs.Add(dronePrefab);
        referencedPrefabs.Add(storePrefab);
    }
}