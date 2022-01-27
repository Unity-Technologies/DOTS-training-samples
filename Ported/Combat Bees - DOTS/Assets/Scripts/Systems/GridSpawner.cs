using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Spawns prefabs on a grid.
/// The grid is offset so that it is centered around the spawner position.
/// </summary>
public partial class GridSpawner : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref GridSpawnerData spawner, in GridDimensions gridDimensions) =>
            {
                float halfCellSize = gridDimensions.CellSize / 2;
                
                for (int i = 0; i < spawner.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(spawner.PrefabToSpawn);
                    var translation = new Translation();

                    int gridCellX = spawner.Random.NextInt(0, gridDimensions.CellsX);
                    int gridCellZ = spawner.Random.NextInt(0, gridDimensions.CellsZ);

                    float centeringOffsetX = gridDimensions.CellsX * gridDimensions.CellSize / 2;
                    float centeringOffsetZ = gridDimensions.CellsZ * gridDimensions.CellSize / 2;

                    translation.Value = new float3(
                        spawner.Position.x + gridCellX * gridDimensions.CellSize - centeringOffsetX,
                        spawner.Position.y,
                        spawner.Position.z + gridCellZ * gridDimensions.CellSize - centeringOffsetZ) + halfCellSize;
                    
                    ecb.SetComponent(prefabInstance, translation);
                    ecb.RemoveComponent<GridSpawnerData>(entity); // Prevents repeated spawning
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}