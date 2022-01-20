using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Spawns prefabs on a grid.
/// The grid is offset so that it is centered around (x: 0, y: 0, z: 0)
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
            .ForEach((Entity entity, ref Spawner spawner, in GridDimensions gridDimensions) =>
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
                        gridCellX * gridDimensions.CellSize - centeringOffsetX,
                        0,
                        gridCellZ * gridDimensions.CellSize - centeringOffsetZ) + halfCellSize;
                    
                    ecb.SetComponent(prefabInstance, translation);
                    ecb.RemoveComponent<Spawner>(entity); // Prevents repeated spawning
                }
            }).WithoutBurst().Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}