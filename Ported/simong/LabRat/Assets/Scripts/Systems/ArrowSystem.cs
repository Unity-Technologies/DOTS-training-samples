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

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        int maxArrows = ConstantData.Instance.MaxArrows;
        double time = Time.ElapsedTime;

        Entity arrowPrefab = GetSingleton<PrefabReferenceComponent>().ArrowPrefab;
        
        var arrowComponents = m_ArrowsQuery.ToComponentDataArrayAsync<ArrowComponent>(Allocator.TempJob, out var arrowComponentsHandle);
        var arrowEntities = m_ArrowsQuery.ToEntityArrayAsync(Allocator.TempJob, out var arrowEntitiesHandle);
        
        Dependency = JobHandle.CombineDependencies(Dependency, arrowComponentsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arrowEntitiesHandle);
        
        var ecb = m_ECBSystem.CreateCommandBuffer();
        
        Entities
            .WithDeallocateOnJobCompletion(arrowComponents)
            .WithDeallocateOnJobCompletion(arrowEntities)
           .ForEach((Entity arrowRequestEntity, ArrowRequest request) =>
            {
                int existingComponents = 0;
                int oldestIndex = -1;
                double oldestSpawnTime = double.MaxValue;
                bool shouldSpawn = true;
                for (int i = 0; i < arrowComponents.Length; i++)
                    if (arrowComponents[i].OwnerID == request.OwnerID)
                    {
                        existingComponents++;
                        if (arrowComponents[i].SpawnTime < oldestSpawnTime)
                        {
                            oldestSpawnTime = arrowComponents[i].SpawnTime;
                            oldestIndex = i;
                        }

                        if (arrowComponents[i].GridCell.x == request.Position.x && arrowComponents[i].GridCell.y == request.Position.y)
                        {
                            shouldSpawn = false;
                        }
                    }

                if (shouldSpawn)
                {
                    if (existingComponents >= maxArrows && oldestIndex >= 0)
                    {
                        ecb.DestroyEntity(arrowEntities[oldestIndex]);
                    }

                    Entity spawnedArrow = ecb.Instantiate(arrowPrefab);
                    if (spawnedArrow != Entity.Null)
                    {
                        ecb.SetComponent(spawnedArrow, new Position2D {Value = new float2(0f, 0f)});
                        ecb.SetComponent(spawnedArrow, new Direction2D {Value = request.Direction});
                        ecb.SetComponent(spawnedArrow,
                            new ArrowComponent
                                {GridCell = request.Position, SpawnTime = time, OwnerID = request.OwnerID});
                    }
                }

                ecb.DestroyEntity(arrowRequestEntity);
            })
            .WithName("CollisionSystem")
            .Schedule();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
