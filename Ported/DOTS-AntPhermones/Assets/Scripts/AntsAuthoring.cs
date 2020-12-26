using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Material = UnityEngine.Material;
using Unity.Rendering;
using UnityEngine.Experimental.Rendering;
using Unity.Mathematics;

public partial class AntsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
	public Camera observerCamera;
	public GameObject antPrefab;
	public GameObject obstaclePrefab;
	public GameObject resourcePrefab;
	public GameObject colonyPrefab;
	public Material basePheromoneMaterial;
	public Renderer pheromoneRenderer;
	public Color searchColor;
	public Color carryColor;
	public int antCount;
	public int mapSize = 128;
	public float antSpeed;
	[Range(0f, 1f)]
	public float antAccel;
	public float trailAddSpeed;
	[Range(0f, 1f)]
	public float trailDecay;
	public float randomSteering;
	public float pheromoneSteerStrength;
	public float wallSteerStrength;
	public float goalSteerStrength;
	public float outwardStrength;
	public float inwardStrength;
	public int obstacleRingCount;
	[Range(0f, 1f)]
	public float obstaclesPerRing;

	Texture2D pheromoneTexture;
	Material myPheromoneMaterial;

  

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		var obstacleRadius = obstaclePrefab.transform.localScale.x * 0.5f;
		var obstaclePrefabEntity = conversionSystem.GetPrimaryEntity(obstaclePrefab);
		dstManager.AddComponent<ObstacleData>(obstaclePrefabEntity);
		var colonyPrefabEntity = conversionSystem.GetPrimaryEntity(colonyPrefab);
		dstManager.AddComponent<Colony>(colonyPrefabEntity);
		var resourcePrefabEntity = conversionSystem.GetPrimaryEntity(resourcePrefab);
		dstManager.AddComponent<Resource>(resourcePrefabEntity);
		pheromoneTexture = new Texture2D(mapSize, mapSize,GraphicsFormat.R32_SFloat,TextureCreationFlags.None);
		var colors = pheromoneTexture.GetRawTextureData<float>();
        for (int i = 0; i < colors.Length; i++)
        {
			colors[i] = 0;
        }
		pheromoneTexture.Apply();
		pheromoneTexture.wrapMode = TextureWrapMode.Mirror;
		myPheromoneMaterial = new Material(basePheromoneMaterial);
		myPheromoneMaterial.mainTexture = pheromoneTexture;
		pheromoneRenderer.sharedMaterial = myPheromoneMaterial;
		pheromoneRenderer.transform.localScale = new Vector3(mapSize, mapSize, 1);
		var mapCenter = Vector2.one * mapSize * 0.5f;
		pheromoneRenderer.transform.localPosition = mapCenter;
		var cameraDistance = 0.9129055f * mapSize;
		observerCamera.transform.localPosition = new Vector3(mapCenter.x,mapCenter.y, -cameraDistance);
		observerCamera.farClipPlane = cameraDistance + 5;
		observerCamera.nearClipPlane = math.max(0.01f, cameraDistance - 5);
		var antPrefabEntity = conversionSystem.GetPrimaryEntity(antPrefab);
		dstManager.AddComponent<AntData>(antPrefabEntity);
		dstManager.AddBuffer<NearbyObstacle>(antPrefabEntity);
		dstManager.AddComponent<MaterialColor>(antPrefabEntity);
		dstManager.AddComponent<Excitement>(antPrefabEntity);
		dstManager.AddComponentData(entity, new AntParams()
		{
			AntSpeed = antSpeed,
			RandomSteering = randomSteering,
			MapSize = mapSize,
			PheromoneSteerStrength = pheromoneSteerStrength,
			WallSteerStrength = wallSteerStrength,
			AntAccel = antAccel,
			SearchColor = (Vector4)searchColor,
			CarryColor = (Vector4)carryColor,
			GoalSteerStrength = goalSteerStrength,
			ObstacleRadius = obstacleRadius,
			OutwardStrength = outwardStrength,
			InwardStrength = inwardStrength,
			TrailAddSpeed = trailAddSpeed,
		});
		dstManager.AddSharedComponentData(entity, new PheromoneTexture() { Value = pheromoneTexture });
		dstManager.AddComponentData(entity, new InitializeAnts()
		{
			AntCount = antCount,
			AntPrefab = antPrefabEntity,
			ObstaclePrefab = obstaclePrefabEntity,
			ColonyPrefab = colonyPrefabEntity,
			ResourcePrefab = resourcePrefabEntity,
			ObstacleRingCount = obstacleRingCount,
			ObstaclesPerRing = obstaclesPerRing,
			RandomSeed = (uint)UnityEngine.Random.value.GetHashCode(),
		});
	}
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
		referencedPrefabs.Add(antPrefab);
		referencedPrefabs.Add(obstaclePrefab);
		referencedPrefabs.Add(resourcePrefab);
		referencedPrefabs.Add(colonyPrefab);
    }

	
}

