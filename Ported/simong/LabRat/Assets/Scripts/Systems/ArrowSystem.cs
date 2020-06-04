using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ArrowSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private EntityQuery m_ArrowsQuery;
    
    protected override void OnCreate()
    {
       base.OnCreate();
       
        m_ArrowsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<ArrowComponent>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    private NativeArray<Entity> m_arrowPrefabs = new NativeArray<Entity>(4, Allocator.Persistent);
    private bool m_initedArrowPrefabs = false;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_arrowPrefabs.Dispose();
    }

    protected override void OnUpdate()
    {
        var gridSystem = World.GetExistingSystem<GridCreationSystem>();
        if (gridSystem == null || !gridSystem.Cells.IsCreated)
            return;
        
        int maxArrows = ConstantData.Instance.MaxArrows;
        float2 cellSize = new float2(ConstantData.Instance.CellSize);
        double time = Time.ElapsedTime;

        if (m_initedArrowPrefabs == false)
        {
            m_arrowPrefabs[0] = GetSingleton<PrefabReferenceComponent>().ArrowPrefab0;
            m_arrowPrefabs[1] = GetSingleton<PrefabReferenceComponent>().ArrowPrefab1;
            m_arrowPrefabs[2] = GetSingleton<PrefabReferenceComponent>().ArrowPrefab2;
            m_arrowPrefabs[3] = GetSingleton<PrefabReferenceComponent>().ArrowPrefab3;
            m_initedArrowPrefabs = true;
        }

        var arrowPrefabs = m_arrowPrefabs;

        var arrowComponents = m_ArrowsQuery.ToComponentDataArrayAsync<ArrowComponent>(Allocator.TempJob, out var arrowComponentsHandle);
        var arrowEntities = m_ArrowsQuery.ToEntityArrayAsync(Allocator.TempJob, out var arrowEntitiesHandle);
        
        Dependency = JobHandle.CombineDependencies(Dependency, arrowComponentsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arrowEntitiesHandle);
        
        var ecb = m_ECBSystem.CreateCommandBuffer();
        
        var cells = gridSystem.Cells;
        var rows = ConstantData.Instance.BoardDimensions.x;
        
        Entities
            .WithDeallocateOnJobCompletion(arrowComponents)
            .WithDeallocateOnJobCompletion(arrowEntities)
            .WithReadOnly(cells)
            .ForEach((Entity arrowRequestEntity, ArrowRequest request) =>
            {
                int existingComponents = 0;
                int oldestIndex = -1;
                double oldestSpawnTime = double.MaxValue;
                bool shouldSpawn = true;
                
                var cellIndex = (request.Position.y * rows) + request.Position.x;
                if (cells[cellIndex].IsBase())
                    shouldSpawn = false;

                for (int i = 0; i < arrowComponents.Length; i++)
                {
                    if (arrowComponents[i].OwnerID == request.OwnerID)
                    {
                        existingComponents++;
                        if (arrowComponents[i].SpawnTime < oldestSpawnTime)
                        {
                            oldestSpawnTime = arrowComponents[i].SpawnTime;
                            oldestIndex = i;
                        }
                    }

                    if (arrowComponents[i].GridCell.x == request.Position.x && arrowComponents[i].GridCell.y == request.Position.y)
                    {
                        shouldSpawn = false;

                        if (arrowComponents[i].OwnerID == request.OwnerID)
                        {
                            ecb.DestroyEntity(arrowEntities[i]);
                            break;
                        }
                    }
                }

                if (shouldSpawn)
                {
                    if (existingComponents >= maxArrows && oldestIndex >= 0)
                    {
                        ecb.DestroyEntity(arrowEntities[oldestIndex]);
                    }

                    Entity spawnedArrow = ecb.Instantiate(arrowPrefabs[request.OwnerID]);
                    if (spawnedArrow != Entity.Null)
                    {
                        ecb.SetComponent(spawnedArrow, new Position2D {Value = Utility.GridCoordinatesToWorldPos(request.Position, cellSize)});
                        ecb.SetComponent(spawnedArrow, new Direction2D {Value = request.Direction});
                        ecb.SetComponent(spawnedArrow, new Rotation2D { Value = Utility.DirectionToAngle(request.Direction) });
                        ecb.SetComponent(spawnedArrow, new ArrowComponent {GridCell = request.Position, SpawnTime = time, OwnerID = request.OwnerID});
                    }
                }

                ecb.DestroyEntity(arrowRequestEntity);
            })
            .WithName("CollisionSystem")
            .Schedule();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
