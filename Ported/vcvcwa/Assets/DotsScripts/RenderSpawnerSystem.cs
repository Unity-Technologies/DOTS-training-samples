using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class RenderSpawnerSystem : JobComponentSystem
{
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    //
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    //
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    private EntityQuery spawnerQuery;

    
    private EntityQuery gridQuery;
    private DynamicBuffer<GridTile> grid;
    private int gridSize;
    
    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        EntityQueryDesc queryDescription = new EntityQueryDesc();
        queryDescription.All = new[] {ComponentType.ReadOnly<GridTile>(), ComponentType.ReadOnly<GridComponent>()};
        gridQuery = GetEntityQuery(queryDescription);
        
        spawnerQuery = GetEntityQuery(ComponentType.ReadOnly<RenderSpawner>());
    }

    // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance
    // Add (CompileSynchronously = true) in these cases
    [BurstCompile(CompileSynchronously = true)]
    struct SpawnJob : IJobChunk
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<RenderSpawner> RenderSpawnerType;
        [ReadOnly] public DynamicBuffer<GridTile> Grid;
        public int GridSize;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkSpawners = chunk.GetNativeArray(RenderSpawnerType);
            var chunkEntities = chunk.GetNativeArray(EntityType);
            if( chunk.Count > 0)
            {
                var spawner = chunkSpawners[0];

                for (var i = 0; i < GridSize; ++i)
                {
                    for (var j = 0; j < GridSize; ++j)
                    {
                        var x = i * spawner.TileSize;
                        var y = j * spawner.TileSize;

                        var instance = CommandBuffer.Instantiate(chunkIndex, spawner.landPrefab);
                        
                        // Place the instantiated in a grid with some noise
                        var position = new float3(x,0,y);
                        CommandBuffer.AddComponent<TileRenderer>(chunkIndex, instance);
                        CommandBuffer.SetComponent(chunkIndex, instance, new Translation {Value = position});
                        CommandBuffer.SetComponent(chunkIndex, instance, new TileRenderer {tile = new int2(i,j)});
                        
                        // do the rest
                        int gridIndex = i * GridSize + j;
                        if (gridIndex < Grid.Length)
                        {
                            var gridElement = Grid[gridIndex];

                            position.y += 1.0f;
                            
                            if (gridElement.IsShop())
                            {
                                var shopInstance = CommandBuffer.Instantiate(chunkIndex, spawner.shopPrefab);
                                CommandBuffer.AddComponent<TileRenderer>(chunkIndex, shopInstance);
                                CommandBuffer.SetComponent(chunkIndex, shopInstance, new Translation {Value = position});
                                CommandBuffer.SetComponent(chunkIndex, shopInstance, new TileRenderer {tile = new int2(i, j)});
                            }
                            
                            if (gridElement.IsRockOrigin())
                            {
                                var rockInstance = CommandBuffer.Instantiate(chunkIndex, spawner.rockPrefab);
                                CommandBuffer.AddComponent<TileRenderer>(chunkIndex, rockInstance);
                                CommandBuffer.AddComponent<NonUniformScale>(chunkIndex, rockInstance);
                                CommandBuffer.AddComponent<ScaledRenderer>(chunkIndex, rockInstance);
                                
                                CommandBuffer.SetComponent(chunkIndex, rockInstance, new Translation {Value = position});
                                CommandBuffer.SetComponent(chunkIndex, rockInstance, new TileRenderer {tile = new int2(i, j)});
                                CommandBuffer.SetComponent(chunkIndex, rockInstance, new ScaledRenderer
                                {
                                    Max = 25,
                                    XZScaleAtZero = 0.25f,
                                    XZScaleAtMax = 4.75f,
                                    YScaleAtZero = 1.0f,
                                    YScaleAtMax = 1.0f,
                                });
                            }
                        }
                            
                    }
                }
            }

            for (int i = 0; i < chunk.Count; ++i)
            {
                CommandBuffer.DestroyEntity(chunkIndex, chunkEntities[i]);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        if (spawnerQuery.CalculateEntityCount() <= 0)
        {
            return inputDependencies;
        }
        
        // Bug - dynamic buffers get blown away by the entity debugger - Daniel
        //if (!grid.IsCreated)
        {
            var gridEntity = gridQuery.GetSingletonEntity();
            grid = EntityManager.GetBuffer<GridTile>(gridEntity);
            
            var gridComponent = EntityManager.GetComponentData<GridComponent>(gridEntity);
            gridSize = gridComponent.Size;

            Assert.IsTrue(grid.IsCreated);
        }

        var job = new SpawnJob()
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType(),
            RenderSpawnerType = GetArchetypeChunkComponentType<RenderSpawner>(true),
            Grid = grid,
            GridSize = gridSize
        }.Schedule(spawnerQuery, inputDependencies);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}
