#define DONT_EAT_ENTITIES
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UpdateScoreSystem : SystemBase
{
    EntityCommandBufferSystem CommandBufferSystem;
    NativeArray<int> ScoreCache;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameStartedTag>();
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        ScoreCache = new NativeArray<int>(4, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        ScoreCache.Dispose();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer();
        var boardEntity = GetSingletonEntity<BoardDefinition>();
        var playerReference = GetBufferFromEntity<PlayerReference>(true)[boardEntity];

        var localScoreCache = ScoreCache;

        for (int i = 0; i < 4; ++i)
        {
            var playerEntity = playerReference[i].Player;
            localScoreCache[i] = EntityManager.GetComponentData<Score>(playerEntity).Value;
        }
        Entities.WithNone<CatTag>().ForEach((Entity e, in HittingGoal goalInfo) =>
        {
            ++localScoreCache[goalInfo.PlayerIndex];
            #if !DONT_EAT_ENTITIES
            ecb.DestroyEntity(e);
            #endif
        }).Run();

        Entities.WithAll<CatTag>().WithAll<HittingGoal>().ForEach((Entity e) =>
        {
            #if !DONT_EAT_ENTITIES
            ecb.DestroyEntity(e);
            #endif
        }).Run();

        for (int i = 0; i < 4; ++i)
        {
            var playerEntity = playerReference[i].Player;
            EntityManager.SetComponentData(playerEntity, new Score(){Value = localScoreCache[i]});
        }
    }
}
