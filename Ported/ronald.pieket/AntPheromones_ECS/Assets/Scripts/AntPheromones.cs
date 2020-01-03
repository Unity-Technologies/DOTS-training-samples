using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;

public class AntPheromones : MonoBehaviour
{
  [Header("Set in inspector")]
  public GameObject AntPrefab;
  public GameObject ObstaclePrefab;
  public GameObject ColonyPrefab;
  public GameObject ResourcePrefab;
  public Renderer PheromoneRenderer;

  public int AntCount = 100;
  public int MapSize = 128;
  public int BucketResolution = 64;
  public float RandomSteerStrength = .1f;
  public float PheromoneSteerStrength = .04f;
  public float WallSteerStrength = .12f;
  public float GoalSteerStrength = .04f;
  public float OutwardStrength = .0003f;
  public float InwardStrength = .0003f;
  public float DecayRate = .999f;
  public float DropRate = .005f;
  public float ObstacleRadius = 2f / 128;
  public float ObstacleRingCount = 3;
  public float ObstaclesPerRing = .8f;
  public float TargetRadius = 4f / 128;

  Texture2D PheromoneTexture;
  Material MyPheromoneMaterial;
  public Material basePheromoneMaterial;
  private PheromonesToTextureSystem MyPheromonesToTextureSystem;
  private Color[] Pixels;

  void Update()
  {
    MyPheromonesToTextureSystem.Handle.Complete();
    var pixels = MyPheromonesToTextureSystem.Pixels;
    if (pixels.IsCreated)
    {
      pixels.CopyTo(Pixels);
      PheromoneTexture.SetPixels(Pixels);
      PheromoneTexture.Apply();
    }
  }

  void Awake()
  {
    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

    // Set up the config entity
    ConfigComponent config = new ConfigComponent()
    {
      MapSize = MapSize,
      BucketResolution = BucketResolution,
      AntCount = AntCount,
      RandomSteerStrength = RandomSteerStrength,
      PheromoneSteerStrength = PheromoneSteerStrength,
      WallSteerStrength = WallSteerStrength,
      GoalSteerStrength = GoalSteerStrength,
      OutwardStrength = OutwardStrength,
      InwardStrength = InwardStrength,
      DecayRate = DecayRate,
      DropRate = DropRate,
      ObstacleRadius = ObstacleRadius,
      ObstacleRingCount = ObstacleRingCount,
      ObstaclesPerRing = ObstaclesPerRing,
      TargetRadius = TargetRadius,
    };

    MyPheromonesToTextureSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PheromonesToTextureSystem>();

    PheromoneTexture = new Texture2D(config.MapSize,config.MapSize);
    PheromoneTexture.wrapMode = TextureWrapMode.Mirror;
    MyPheromoneMaterial = new Material(basePheromoneMaterial);
    MyPheromoneMaterial.mainTexture = PheromoneTexture;
    PheromoneRenderer.sharedMaterial = MyPheromoneMaterial;
    Pixels = new Color[config.MapSize * config.MapSize];

    {
      var entity = entityManager.CreateEntity();
      entityManager.AddComponentData(entity, config);
    }

    Gameboard.Initialize();
    var gameboard = Gameboard.CreateGameboardComponent(config);
    {
      var entity = entityManager.CreateEntity();
      entityManager.AddComponentData(entity, gameboard);
    }

    // Create the pheromones DynamicBuffer
    {
      var entity = entityManager.CreateEntity();
      var pheromones = new NativeArray<PheromoneElement>(config.MapSize * config.MapSize, Allocator.Persistent);
/*
      // Draw a large X with pheromones, to verify that pheromone steering is working
      PheromoneComponent one = new PheromoneComponent(){ Value = 1f };
      for (int i = 0; i < config.MapSize; ++i)
      {
        var j = config.MapSize - 1 - i;
        pheromones[i + i * config.MapSize] = one;
        pheromones[j + i * config.MapSize] = one;
        if (i > 0)
        {
          pheromones[i + i * config.MapSize - 1] = one;
          pheromones[j + i * config.MapSize + 1] = one;
        }

        if (i < config.MapSize - 1)
        {
          pheromones[i + i * config.MapSize + 1] = one;
          pheromones[j + i * config.MapSize - 1] = one;
        }
      }
*/
      var pheromonesBuffer = entityManager.AddBuffer<PheromoneElement>(entity);
      pheromonesBuffer.AddRange(pheromones);
      pheromones.Dispose();
    }

    // Create entities for the ants
    var antPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(AntPrefab, World.DefaultGameObjectInjectionWorld);
    for (int i = 0; i < config.AntCount; ++i)
    {
      var instance = entityManager.Instantiate(antPrefabEntity);
      float f = 5f / config.MapSize;
      var position = new float3(UnityEngine.Random.Range(-f, f) + .5f,UnityEngine.Random.Range(-f, f) + .5f, 0f);
      entityManager.AddComponentData(instance, new Translation {Value = position});
      entityManager.AddComponentData(instance, new Rotation {Value = Quaternion.identity});
      entityManager.AddComponentData(instance, new HeadingComponent() {Value = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI)});
      entityManager.AddComponentData(instance, new SpeedComponent() {Value = .025f});
      entityManager.AddComponentData(instance, new VelocityComponent() {Value = new float2(0, 0)});
      entityManager.AddComponentData(instance, new TargetComponent() {Value = gameboard.ResourcePosition});
      entityManager.AddComponentData(instance, new ResourceComponent() {HasResource = false});
      entityManager.AddComponentData(instance, new AntTag());
    }

    // Create entities to render the obstacles
    var obstaclePrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ObstaclePrefab, World.DefaultGameObjectInjectionWorld);
    for (int i = 0; i < gameboard.ObstacleList.Value.Positions.Length; ++i)
    {
      float2 v = gameboard.ObstacleList.Value.Positions[i];
      var instance = entityManager.Instantiate(obstaclePrefabEntity);
      entityManager.AddComponentData(instance, new Translation {Value = new Vector3(v.x, v.y, 0)});
    }

    // Create entity to render the colony
    {
      var colonyPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ColonyPrefab, World.DefaultGameObjectInjectionWorld);
      float2 v = gameboard.ColonyPosition;
      var instance = entityManager.Instantiate(colonyPrefabEntity);
      entityManager.AddComponentData(instance, new Translation {Value = new Vector3(v.x, v.y, 0)});
    }

    // Create entity to render the resource
    {
      var resourcePrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ResourcePrefab, World.DefaultGameObjectInjectionWorld);
      float2 v = gameboard.ResourcePosition;
      var instance = entityManager.Instantiate(resourcePrefabEntity);
      entityManager.AddComponentData(instance, new Translation {Value = new Vector3(v.x, v.y, 0)});
    }
  }
}
