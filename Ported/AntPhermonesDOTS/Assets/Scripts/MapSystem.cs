using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct MapInitializedTag : IComponentData { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class MapSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;
    private EntityQuery TileInitQuery;
    
    Texture2D PheromoneTexture;
    private NativeArray<Color> colors;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        TileInitQuery = GetEntityQuery(ComponentType.ReadWrite<MapInitializedTag>());
        RequireSingletonForUpdate<Map>();
    }

    protected override void OnDestroy()
    {
        colors.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var commandBuffer = EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        int numCores = System.Environment.ProcessorCount;
        Map map = GetSingleton<Map>();
        int elementsPerTile = (map.Size * map.Size) / numCores;
        
        if (PheromoneTexture == null)
        {
            PheromoneTexture = new Texture2D(map.Size, map.Size);
            PheromoneTexture.wrapMode = TextureWrapMode.Mirror;
            colors = new NativeArray<Color>(map.Size * map.Size, Allocator.Persistent);
            //Material.mainTexture = PheromoneTexture;
        }
        
        // Generate computation tiles.
        var tileJobHandle = new JobHandle();
        if (TileInitQuery.IsEmptyIgnoreFilter)
        {
            tileJobHandle = Entities
                .WithName("MapToTilesSystem")
                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                .ForEach((Entity mapEntity, int entityInQueryIndex) =>
                {
                    for (var i = 0; i < numCores; i++)
                    {
                        var entity = commandBuffer.CreateEntity(entityInQueryIndex);
                        commandBuffer.AddComponent<Tile>(entityInQueryIndex, entity, new Tile {Coordinates = new int2(i * elementsPerTile, 0)});
                    }

                    commandBuffer.AddComponent<MapInitializedTag>(entityInQueryIndex, mapEntity);
                }).Schedule(inputDeps);

            EntityCommandBufferSystem.AddJobHandleForProducer(tileJobHandle);
        }
        var combined = JobHandle.CombineDependencies(inputDeps, tileJobHandle);
        
        // Jobs write colors into tiles.
        var colorArray = colors;
        var updateColorsJobHandle = Entities
            .WithName("UpdateMapSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in Tile tile) =>
            {
                int begin = tile.Coordinates.x;
                int end = tile.Coordinates.x + elementsPerTile;
                for (int i = begin; i < end; ++i)
                {
                    colorArray[i] = new Color(entityInQueryIndex * 1.33f, i / elementsPerTile, i);
                }
            }).Schedule(combined);
        updateColorsJobHandle.Complete();

        PheromoneTexture.SetPixels(colorArray.ToArray());
        PheromoneTexture.Apply();
        
        return JobHandle.CombineDependencies(combined, updateColorsJobHandle);
    }
}