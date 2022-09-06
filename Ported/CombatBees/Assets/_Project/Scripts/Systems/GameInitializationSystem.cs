using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.Windows;

[AlwaysUpdateSystem]
[AlwaysSynchronizeSystem]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class GameInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Game reset
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R))
        {
            Entity gameInitEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(gameInitEntity, new GameInitialization());
        }
        
        if (HasSingleton<GameInitialization>() &&
            HasSingleton<GameGlobalData>() &&
            HasSingleton<GameSceneData>())
        {
            GameGlobalData globalData = GetSingleton<GameGlobalData>();
            GameSceneData sceneData = GetSingleton<GameSceneData>();
            
            // Scene cleanup
            {
                EntityQuery beesQuery = GetEntityQuery(ComponentType.ReadWrite(typeof(Bee)));
                EntityQuery resourcesQuery = GetEntityQuery(ComponentType.ReadWrite(typeof(Resource)));
                EntityQuery particlesQuery = GetEntityQuery(ComponentType.ReadWrite(typeof(Particle)));

                NativeArray<Entity> beeEntities = beesQuery.ToEntityArray(Allocator.Temp);
                for (int i = 0; i < beeEntities.Length; i++)
                {
                    EntityManager.DestroyEntity(beeEntities[i]);
                }
                beeEntities.Dispose();
                EntityManager.DestroyEntity(resourcesQuery);
                EntityManager.DestroyEntity(particlesQuery);

                if (HasSingleton<GameRuntimeData>())
                {
                    EntityManager.DestroyEntity(GetSingletonEntity<GameRuntimeData>());
                }
            }

            // Create the runtime game data (contains Random + Grid data)
            {
                Entity tmpRuntimeDataEntity = EntityManager.CreateEntity(typeof(GameRuntimeData));
                
                GameRuntimeData tmpRuntimeData = GetComponent<GameRuntimeData>(tmpRuntimeDataEntity);
                tmpRuntimeData.Random = Random.CreateFromIndex(0);
                tmpRuntimeData.GridCharacteristics = new GridCharacteristics(
                    float3.zero, 
                    sceneData.FloorSizeX,
                    sceneData.TeamFloorSizeX, 
                    sceneData.FloorSizeZ, 
                    globalData.GridCellSize,
                    sceneData.TeamZoneHeight);
                
                // Stack counts array
                ResourceSystem resourceSystem = World.GetOrCreateSystem<ResourceSystem>();
                if (resourceSystem.CellResourceStackCount.IsCreated)
                {
                    resourceSystem.CellResourceStackCount.Dispose();
                }
                resourceSystem.CellResourceStackCount = new NativeArray<int>(tmpRuntimeData.GridCharacteristics.GetTotalCellCount(), Allocator.Persistent);
                
                // Add event buffers
                EntityManager.AddBuffer<ResourceSpawnEvent>(tmpRuntimeDataEntity);
                EntityManager.AddBuffer<BeeSpawnEvent>(tmpRuntimeDataEntity);
                EntityManager.AddBuffer<ParticleSpawnEvent>(tmpRuntimeDataEntity);
                
                SetSingleton(tmpRuntimeData);
            }
            
            Entity runtimeDataEntity = GetSingletonEntity<GameRuntimeData>();
            GameRuntimeData runtimeData = GetSingleton<GameRuntimeData>();
            
            // Generate level based on grid data
            {
                int totalCellCount = runtimeData.GridCharacteristics.GetTotalCellCount();

                NativeArray<Entity> levelCubes = new NativeArray<Entity>(totalCellCount, Allocator.Temp);
                EntityManager.Instantiate(globalData.GenericCubePrefab, levelCubes);

                for (int i = 0; i < totalCellCount; i++)
                {
                    Entity cubeEntity = levelCubes[i];
                    int2 cellCoords = runtimeData.GridCharacteristics.GetCoordinatesOfCellIndex(i);
                    CellType cellType = runtimeData.GridCharacteristics.GetTypeOfCell(cellCoords);
                    
                    UnityEngine.Color cellColor = default;
                    switch (cellType)
                    {
                        case CellType.Floor:
                            cellColor = globalData.FloorColor;
                            break;
                        case CellType.TeamA:
                            cellColor = globalData.TeamAColor;
                            break;
                        case CellType.TeamB:
                            cellColor = globalData.TeamBColor;
                            break;
                    }
                        
                    SetComponent(cubeEntity, new Translation { Value = runtimeData.GridCharacteristics.GetPositionOfCell(cellCoords) });
                    SetComponent(cubeEntity, new NonUniformScale() { Value = new float3(globalData.GridCellSize) });
                    SetComponent(cubeEntity, new OverridableMaterial_Color() { Value = GameUtilities.ColorToFloat4(cellColor) });
                }

                levelCubes.Dispose();
            }
            
            // Spawn resources
            {
                DynamicBuffer<ResourceSpawnEvent> resourceSpawnEvents = EntityManager.GetBuffer<ResourceSpawnEvent>(runtimeDataEntity);
                for (int i = 0; i < sceneData.StartResourcesCount; i++)
                {
                    resourceSpawnEvents.Add(new ResourceSpawnEvent
                    {
                        Position = GameUtilities.GetRandomPosInsideBox(ref runtimeData.Random, sceneData.ResourcesSpawnBox),
                    });
                }
                SetSingleton(runtimeData);
            }

            // Spawn bees
            {
                DynamicBuffer<BeeSpawnEvent> beeSpawnEvents = EntityManager.GetBuffer<BeeSpawnEvent>(runtimeDataEntity);
                for (int i = 0; i < sceneData.StartBeesCountPerTeam * 2; i++)
                {
                    bool isTeamB = (i >= sceneData.StartBeesCountPerTeam);
                    
                    beeSpawnEvents.Add(new BeeSpawnEvent
                    {
                        Position = isTeamB ? runtimeData.GridCharacteristics.TeamBBounds.Center : runtimeData.GridCharacteristics.TeamABounds.Center,
                        Team = isTeamB ? Team.TeamB : Team.TeamA,
                    });
                }
                SetSingleton(runtimeData);
            }

            // Destroy initialization entity
            EntityManager.DestroyEntity(GetSingletonEntity<GameInitialization>());
        }
    }
}
