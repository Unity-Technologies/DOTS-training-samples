using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class DynamicSpawnerSystem : SystemBase
{
    EntityQuery m_CatQuery;

    protected override void OnCreate()
    {
        // Query to grab the cats (need max number of cats)
        m_CatQuery = GetEntityQuery(typeof(CatTag), typeof(GridPosition), typeof(CellOffset), typeof(Direction));
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Figure out number of cats
        var catEntities = m_CatQuery.ToEntityArray(Allocator.Temp);
        var numberOfCats = catEntities.Length;

        // Need to access to cat and mouse prefabs
        var prefabsSingleton = GetSingleton<BoardPrefab>();

        var dt = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref SpawnerData spawnerData) =>
            {
                spawnerData.Timer -= dt;

                if (spawnerData.Timer < 0 && !(spawnerData.Type == SpawnerType.CatSpawner && numberOfCats >= 8)) //TODO: Number of cats
                {
                    spawnerData.Timer = spawnerData.Frequency;

                    // Choose the prefab
                    var prefab = prefabsSingleton.MousePrefab;
                    if (spawnerData.Type == SpawnerType.CatSpawner)
                    {
                        prefab = prefabsSingleton.CatPrefab;
                    }

                    // Create the prefab
                    var spawnedEntity = ecb.Instantiate(prefab);

                    // Set its components
                    var gridPosition = new GridPosition() { X = spawnerData.X, Y = spawnerData.Y };
                    var direction = new Direction() { Value = spawnerData.Direction };
                    var cellOffset = new CellOffset() { Value = 0.5f };
                    ecb.AddComponent(spawnedEntity, gridPosition);
                    ecb.AddComponent(spawnedEntity, direction);
                    ecb.AddComponent(spawnedEntity, cellOffset);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
