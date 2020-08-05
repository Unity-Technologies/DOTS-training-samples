using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class LevelGenerationSystem : SystemBase
{
    //hardcoded for now, be moved later
    public struct LevelSpawner : IComponentData
    {
        public int mapSize;
        public int obstacleRingCount;
        public float obstaclesPerRing;
        public float obstacleRadius;
    }
    public LevelSpawner m_Spawner = new LevelSpawner()
    {
        mapSize = 126,
        obstacleRingCount = 3,
        obstaclesPerRing = 0.8f,
        obstacleRadius = 2
    };
    
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in ObstacleGeneratorAuthoring spawner, in MapResolution map, in LocalToWorld ltw) =>
            {
                //load / create texture with 2 channels (channel resolution to define)
                Texture2D texture = new Texture2D(map.value, map.value, UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32_SInt, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
                //if loaded reset the size

                Color obstacleColor = new Color32(0, 1, 0, 0);

                for (int i = 1; i <= spawner.obstacleRingCount; i++)
                {
                    float ringRadius = (i / (spawner.obstacleRingCount + 1f)) * (map.value * .5f);
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
                            float posX = map.value * .5f + Mathf.Cos(angle) * ringRadius;
                            float posY = map.value * .5f + Mathf.Sin(angle) * ringRadius;

                            //to check: clamping to integer
                            Disc(texture, (int)math.ceil(posX), (int)math.ceil(posY), (int)spawner.obstacleRadius, obstacleColor);
                        }
                    }
                }

                texture.Apply();

                EntityManager.DestroyEntity(entity);
            }).Run();
    }

    //todo: proper imlplementation based on bresenham path on from two ends on half brensenham circles
    static void Disc(Texture2D tex, int cx, int cy, int r, Color col)
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
}
