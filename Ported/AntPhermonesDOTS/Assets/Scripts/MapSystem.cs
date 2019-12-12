using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public struct MapInitializedTag : IComponentData { }

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class MapSystem : JobComponentSystem
{
    BeginInitializationEntityCommandBufferSystem EntityCommandBufferSystem;
    private EntityQuery TileInitQuery;
    private EntityQuery RendererQuery;
    Texture2D PheromoneTexture;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        TileInitQuery = GetEntityQuery(ComponentType.ReadWrite<MapInitializedTag>());
        RendererQuery = GetEntityQuery(ComponentType.ReadWrite<RenderMesh>());
        RequireSingletonForUpdate<Map>();
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
                        int index = i * elementsPerTile;
                        commandBuffer.AddComponent<Tile>(entityInQueryIndex, entity, new Tile {Coordinates = new int2(index, index + elementsPerTile)});
                    }
                    commandBuffer.AddBuffer<PheromoneBufferElement>(entityInQueryIndex, mapEntity);
                    commandBuffer.AddComponent<MapInitializedTag>(entityInQueryIndex, mapEntity);
                }).Schedule(inputDeps);

            EntityCommandBufferSystem.AddJobHandleForProducer(tileJobHandle);
        }
        var combined = JobHandle.CombineDependencies(inputDeps, tileJobHandle);

        // Jobs write colors into tiles.
        if (TileInitQuery.IsEmptyIgnoreFilter == false)
        {
            Entity mapSingletonEntity = GetSingletonEntity<Map>();
            DynamicBuffer<PheromoneBufferElement> pheromones = EntityManager.GetBuffer<PheromoneBufferElement>(mapSingletonEntity);
            NativeArray<Color> colors = new NativeArray<Color>(pheromones.Length, Allocator.TempJob);
            if (pheromones.IsCreated && pheromones.Length > 0)
            {
                var updateColorsJobHandle = Entities
                    .WithName("UpdateMapSystem")
                    .WithReadOnly(pheromones)
                    .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
                    .ForEach((Entity entity, int entityInQueryIndex, in Tile tile) =>
                    {
                        int begin = tile.Coordinates.x;
                        int end = tile.Coordinates.y;
                        for (int x = begin; x < end; ++x)
                        {
                            var bufferElement = pheromones[x];
                            colors[x] = new Color(bufferElement.Value, 0.0f, 0.0f);
                        }
                    }).Schedule(combined);
                updateColorsJobHandle.Complete();

                PheromoneTexture.SetPixels(colors.ToArray());
                PheromoneTexture.Apply();
            }
            colors.Dispose();
        }

        if (RendererQuery.IsEmptyIgnoreFilter == false)
        {
            Entities.WithoutBurst().ForEach((Entity entity, in RenderMesh renderMesh, in Map mapReadonly) =>
            {
                renderMesh.material.mainTexture = PheromoneTexture;
                renderMesh.material.SetTexture("_UnlitColorMap", PheromoneTexture);
            }).Run();
        }

        return combined;
    }
}