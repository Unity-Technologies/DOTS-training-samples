using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    public static readonly float4 MIN_HEIGHT_COLOR = new float4(0, 1, 0, 1);
    public static readonly float4 MAX_HEIGHT_COLOR = new float4(99 / 255f, 47 / 255f, 0 / 255f, 1);

    public const float HEIGHT_MIN = .5f;
    public static readonly float maxTerrainHeight = 10.0f; // TODO: get from config instead

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        var heightMapEntity = EntityManager.CreateEntity(typeof(HeightBufferElement));
        var heightMap = EntityManager.GetBuffer<HeightBufferElement>(heightMapEntity);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                // build terrain
                for (int i = 0; i < spawner.TerrainLength; ++i)
                {
                    for (int j = 0; j < spawner.TerrainWidth; ++j)
                    {
                        float height = random.NextFloat(spawner.MinTerrainHeight, spawner.MaxTerrainHeight);
                        heightMap.Add((HeightBufferElement) height);

                        var box = ecb.Instantiate(spawner.BoxPrefab);
                        ecb.SetComponent(box, new NonUniformScale
                        {
                            Value = new float3(1, height, 1)
                        });

                        ecb.SetComponent(box, new URPMaterialPropertyBaseColor
                        {
                            Value = GetColorForHeight(height, spawner.MaxTerrainHeight)
                        });

                        ecb.SetComponent(box, new Translation
                        {
                            Value = new float3(i, height /  2, j) // reposition halfway to heigh to level all at 0 plane
                        });
                    }
                }

                // new fresh player at proper x/y and height and empty parabola t value
                var player = ecb.Instantiate(spawner.PlayerPrefab);
                int boxX = spawner.TerrainLength / 2; // TODO: spawn player at which box ??? look at original game logic I guess; assuming center for now
                int boxY = spawner.TerrainWidth / 2;
                float boxHeight = heightMap[boxY * spawner.TerrainLength + boxX] + Player.Y_OFFSET; // use box height map to figure out starting y
                ecb.SetComponent(player, new Translation
                {
                    Value = new float3(boxX, boxHeight + Player.Y_OFFSET, boxY) // reposition halfway to heigh to level all at 0 plane
                });
                ecb.AddComponent(player, new ParabolaTValue
                {
                    Value = -1 // less than zero will trigger recalculate the next bounce parabola
                });

                // make tanks!
                // TODO: spawn tanks (barrels/turrets too?) at whatever random logic the original game did
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    /// <summary>
    /// Helper function to calculate terrain color to use for a given height
    /// TODO: how/where do we put to share between spawner and ball collision system when it recalculates color based on new height
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <param name="maxTerrainHeight"></param>
    /// <returns></returns>
    public static float4 GetColorForHeight(float height, float maxTerrainHeight)
    {
        float4 color;

        // change color based on height
        if (math.abs(maxTerrainHeight - HEIGHT_MIN) < math.EPSILON) // special case, if max is close to min just color as min height
        {
            color = MIN_HEIGHT_COLOR;
        }
        else
        {
            color = math.lerp(MIN_HEIGHT_COLOR, MAX_HEIGHT_COLOR, (height - HEIGHT_MIN) / (maxTerrainHeight - HEIGHT_MIN));
        }

        return color;
    }
}
