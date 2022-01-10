using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(SceneSystemGroup))]
public partial class FarmSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<FarmerConfig>();
        RequireSingletonForUpdate<FarmConfig>();
    }

    protected override void OnUpdate()
    {
        var farmConfig = GetSingleton<FarmConfig>();
        var farmerConfig = GetSingleton<FarmerConfig>();
        
        // Create ground tiles
        for (int x = 0; x < farmConfig.MapSizeX; ++x)
        {
            for (int y = 0; y < farmConfig.MapSizeY; ++y)
            {
                var instance = EntityManager.Instantiate(farmConfig.GroundPrefab);
                EntityManager.SetComponentData(instance, new Translation { Value = new float3(x, 0f, y) });
            }
        }

        // Create farmers
        var farmerEntities = EntityManager.Instantiate(farmerConfig.FarmerPrefab, farmerConfig.InitialFarmerCount, Allocator.Temp);
        var random = new Random(1234);
        Entities
            .WithFilter(farmerEntities)
            .ForEach((ref Translation translation) =>
            {
                translation.Value = new float3(random.NextInt(0, farmConfig.MapSizeX), 1f, random.NextInt(0, farmConfig.MapSizeY));
            }).Run();

        EntityManager.AddComponent<CameraFollow>(farmerEntities[0]);


        var gridData = new GridData
        {
            groundTiles = new NativeArray<byte>(farmConfig.MapSizeX * farmConfig.MapSizeY, Allocator.Persistent)
        };
        var farmConfigEntity = GetSingletonEntity<FarmConfig>();
        EntityManager.AddComponentData(farmConfigEntity, gridData);

        Enabled = false;
    }
}