using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

public class LevelGenerationSystem : SystemBase
{
    Texture2D texture;
    int mapSize;
    private float obstacleRingCount;
    private float perlinPersistence;
    private float perlinScale;
    private float perlinThreshold;
    private int perlinOctaves;
    private float perlinAmplitude;
    private bool usePerlinNoise;
    private bool perturbRings;
    private int perlinSeed;

    protected override void OnCreate()
    {
        base.OnCreate();
        var defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
        texture = defaults.colisionMap;
        mapSize = defaults.mapSize;
        perlinPersistence = defaults.perlinPersistence;
        perlinOctaves = defaults.perlinOctaves;
        perlinScale = defaults.perlinScale;
        perlinThreshold = defaults.perlinThreshold;
        perlinAmplitude = defaults.perlinAmplitude;
        usePerlinNoise = defaults.usePerlinNoise;
        perturbRings = defaults.perturbRings;
        perlinSeed = defaults.perlinSeed;
        obstacleRingCount = defaults.obstacleRingCount;
    }

    static float2 SpawnNewFood(int mapSize)
    {
        float2 newRandomPoint = new float2(mapSize / 2.0f, mapSize / 2.0f);
        float2 nestLocation = newRandomPoint;
        float obstacleRingRadius = mapSize * .5f;

        while (Unity.Mathematics.math.length(newRandomPoint - nestLocation) < obstacleRingRadius)
        {
            newRandomPoint = new float2(UnityEngine.Random.Range(0.0f, mapSize), UnityEngine.Random.Range(0.0f, mapSize));
        }
        
        GameObject.Find("Food").GetComponent<Transform>().position = new Vector3(newRandomPoint.x, 0, newRandomPoint.y);

        return newRandomPoint;
    }

    protected override void OnUpdate()
    {


        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in LevelGeneration spawner) =>
            {
                if (texture.width != mapSize || texture.height != mapSize)
                    texture.Resize(mapSize, mapSize);

                Clear(texture);

                if (!usePerlinNoise)
                {
                    for (int i = 1; i <= spawner.obstacleRingCount; i++)
                    {
                        float ringRadius = (i / (spawner.obstacleRingCount + 1f)) * (mapSize * .5f);
                        float circumference = ringRadius * 2f * Mathf.PI;
                        int maxCount = Mathf.CeilToInt(circumference / (2f * spawner.obstacleRadius) * 10f);
                        int offset = UnityEngine.Random.Range(0, maxCount);
                        int holeCount = UnityEngine.Random.Range(1, 3);
                        for (int j = 0; j < maxCount; j++)
                        {
                            float t = (float)j / maxCount;
                            if ((t * holeCount) % 1f < spawner.obstaclesPerRing)
                            {
                                float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                                float posX = mapSize * .5f + Mathf.Cos(angle) * ringRadius;
                                float posY = mapSize * .5f + Mathf.Sin(angle) * ringRadius;

                                if (perturbRings && perlinAmplitude > 0.0f)
                                {
                                    float perlinX = Noise.PerlinNoise((int)posX, (int)posY, perlinScale, perlinOctaves, perlinPersistence, 1.0f, perlinSeed + 98723456);
                                    float perlinY = Noise.PerlinNoise((int)posX, (int)posY, perlinScale, perlinOctaves, perlinPersistence, 1.0f, perlinSeed + 16378623);
                                    posX += (2.0f * perlinX - 1.0f) * perlinAmplitude;
                                    posY += (2.0f * perlinY - 1.0f) * perlinAmplitude;
                                }

                                //to check: clamping to integer
                                DrawDisc(texture, (int)math.ceil(posX), (int)math.ceil(posY), (int)spawner.obstacleRadius, UnityEngine.Color.red);
                            }
                        }
                    }
                }
                else
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        for (int x = 0; x < texture.width; x++)
                        {
                            float perlin = (Noise.PerlinNoise(x, y, perlinScale, perlinOctaves, perlinPersistence, 1.0f, perlinSeed) > perlinThreshold) ? 1.0f : 0.0f;
                            texture.SetPixel(x, y, new UnityEngine.Color(1.0f, 1.0f, 1.0f) * perlin);
                        }
                    }
                }

                texture.Apply();

                var newFoodPosition = SpawnNewFood(mapSize);
                FoodLocation newFoodLocation;
                newFoodLocation.value = newFoodPosition;
                SetSingleton<FoodLocation>(newFoodLocation);

                ColonyLocation colonyLocation;
                colonyLocation.value = new float2(mapSize / 2.0f, mapSize / 2.0f);
                SetSingleton<ColonyLocation>(colonyLocation);
                
                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    static void Clear(Texture2D tex)
    {
        for (int i = 0; i < tex.width; i++)
            for (int j = 0; j < tex.height; j++)
                tex.SetPixel(i, j, default);
    }

    //todo: proper imlplementation based on bresenham path on from two ends on half brensenham circles
    static void DrawDisc(Texture2D tex, int cx, int cy, int r, UnityEngine.Color col)
    {
        int x, y, px, nx, py, ny, d;

        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tex.SetPixel(px, py, col);
                tex.SetPixel(nx, py, col);

                tex.SetPixel(px, ny, col); //to check with 2 channel texture
                tex.SetPixel(nx, ny, col); //to check with 2 channel texture
            }
        }
    }

#if UNITY_EDITOR
    class CreateAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AntDefaults defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
            if (defaults == null || defaults.Equals(null))
            {
                Debug.LogError($"There is no {typeof(AntDefaults)}");
                return;
            }

            //RenderTexture rt = new RenderTexture(defaults.mapSize, defaults.mapSize, 0, RenderTextureFormat.RG32);
            //rt.Create();
            Texture2D rt = new Texture2D(defaults.mapSize, defaults.mapSize, UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            AssetDatabase.CreateAsset(rt, pathName);
            Selection.activeObject = rt;

            if (defaults.colisionMap == null || defaults.colisionMap.Equals(null))
                defaults.colisionMap = rt;
        }
    }

    [MenuItem("Assets/Create/CollisionMap", priority = 0)]
    static void CreateMapMenu()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateAsset>(), "CollisionMap.asset", null, null);
    }
#endif
}
