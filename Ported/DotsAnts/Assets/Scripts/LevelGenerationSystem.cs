using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

public class LevelGenerationSystem : SystemBase
{
    Texture2D texture;
    int mapSize;

    protected override void OnCreate()
    {
        base.OnCreate();
        var defaults = GameObject.Find("Default values").GetComponent<AntDefaults>();
        texture = defaults.colisionMap;
        mapSize = defaults.mapSize;
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in LevelGeneration spawner) =>
            {
                if (texture.width != mapSize || texture.height != mapSize)
                    texture.Resize(mapSize, mapSize);
                
                Clear(texture);

                for (int i = 1; i <= spawner.obstacleRingCount; i++)
                {
                    float ringRadius = (i / (spawner.obstacleRingCount + 1f)) * (mapSize * .5f);
                    float circumference = ringRadius * 2f * Mathf.PI;
                    int maxCount = Mathf.CeilToInt(circumference / (2f * spawner.obstacleRadius) * 2f);
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

                            //to check: clamping to integer
                            DrawDisc(texture, (int)math.ceil(posX), (int)math.ceil(posY), (int)spawner.obstacleRadius, Color.red);
                        }
                    }
                }

                texture.Apply();

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
    static void DrawDisc(Texture2D tex, int cx, int cy, int r, Color col)
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
