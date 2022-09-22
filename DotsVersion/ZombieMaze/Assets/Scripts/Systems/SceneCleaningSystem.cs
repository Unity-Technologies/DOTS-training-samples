using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Jobs;

[BurstCompile]
[WithNone(typeof(MazeConfig))]
[WithNone(typeof(PrefabConfig))]
partial struct CleanAllJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity)
    {
        ECB.DestroyEntity(chunkIndex, entity);
    }
}


[BurstCompile]
partial struct SceneCleaningSystem : ISystem
{
    EntityQuery Query;

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrefabConfig>();
        state.RequireForUpdate<MazeConfig>();

        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<Zombie, PlayerData, RotationComponent>();
        builder.WithAny<MovingWall, Wall>();
        builder.WithNone<MazeConfig, PrefabConfig>();
        Query = state.GetEntityQuery(builder);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        if(mazeConfig.DestroyAllToRemake)
        {
            mazeConfig.DestroyAllToRemake = false;

            state.EntityManager.DestroyEntity(Query);

            mazeConfig.RebuildMaze = true;
            mazeConfig.SpawnPills = true;            
            SystemAPI.SetSingleton<MazeConfig>(mazeConfig);
        }
    }
}
