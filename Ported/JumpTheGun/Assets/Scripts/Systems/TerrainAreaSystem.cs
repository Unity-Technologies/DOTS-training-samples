using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Color = UnityEngine.Color;
public partial class TerrainAreaSystem : SystemBase
{
    public const float SPACING = 1;
    public const float Y_OFFSET = 0;
    public const float HEIGHT_MIN = .5f;
    public static Color MIN_HEIGHT_COLOR = Color.green;
    public static Color MAX_HEIGHT_COLOR = new Color(99 /255f, 47 /255f, 0 /255f);
    
    protected override void OnCreate()
    {
        
    }
    protected override void OnUpdate()
    {
        CreateBoxes();
    }

    public void CreateBoxes(){
        ClearBoxes();
        // Create query for EntityPrefabHolder
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);
        
        
        Entities
            .ForEach((Entity entity, in EntityPrefabHolder prefabHolder, in TerrainData terrainData) =>
            {
                // TODO : Experiment with it
                ecb.DestroyEntity(entity);

                for (int i = 0; i < terrainData.TerrainWidth; ++i)
                {
                    for (int j = 0; j < terrainData.TerrainLength; ++j)
                    {
                        var instance = ecb.Instantiate(prefabHolder.BrickEntityPrefab);
                        ecb.AddComponent(instance, new NonUniformScale());
                        // Set scale
                        float height = random.NextFloat(terrainData.MinTerrainHeight, terrainData.MaxTerrainHeight);
                        float3 scale = new float3(1, height, 1);
                        // Set position
                        float3 pos = new float3(i * SPACING, height / 2 + Y_OFFSET, j * SPACING);
                        ecb.SetComponent(instance, new Translation
                        {
                            Value = pos
                        });
                        ecb.SetComponent(instance, new NonUniformScale
                        {
                            Value = scale
                        });
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    public void ClearBoxes()
    {
        // Create query for all the bricks
        // Destroy
    }
}
