﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
 using Unity.Transforms;
 using UnityEngine;

[UpdateAfter(typeof(SpawningSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CollisionSystem : SystemBase
{
    private EntityQuery m_Query;
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(Cat), typeof(Translation));
        RequireForUpdate(m_Query);
    }
    
    protected override void OnUpdate()
    {
        var catPositionArray = m_Query.ToComponentDataArray<Translation>(Allocator.TempJob);
        var sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var collisionRadius = GetSingleton<GameState>().collisionRadius;

        var tileEntity = GetSingletonEntity<TileMap>();
        TileMap tileMap = EntityManager.GetComponentObject<TileMap>(tileEntity);
        NativeArray<byte> tiles = tileMap.tiles;

        var boardInfo = GetSingleton<BoardInfo>();
        
        Entities
            .WithAll<Mouse>()
            .WithDisposeOnCompletion(catPositionArray)
            .ForEach((ref Entity entity, in Translation translation) =>
        {
            int x = (int)(translation.Value.x + 0.5f);
            int z = (int)(translation.Value.z + 0.5f);
            var tile = TileUtils.GetTile(tiles, x, z, boardInfo.width);
            if (TileUtils.IsHole(tile) || TileUtils.BaseId(tile) ==0)
            {
                ecb.DestroyEntity(entity);
            }
            
            for (var i=0; i<catPositionArray.Length; i++)
            {
                var dist = math.distance(translation.Value, catPositionArray[i].Value);
                if (dist < collisionRadius)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }).Schedule();

        Entities.WithAll<Cat>().ForEach((ref Entity entity, in Translation translation) =>
        {
            int x = (int)(translation.Value.x + 0.5f);
            int z = (int)(translation.Value.z + 0.5f);
            var tile = TileUtils.GetTile(tiles, x, z, boardInfo.width);
            if (TileUtils.IsHole(tile))
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule();
        
        sys.AddJobHandleForProducer(Dependency);
    }
}
