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
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameStartedTag>();
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        var boardEntity = GetSingletonEntity<BoardDefinition>();
        var playerReference = GetBufferFromEntity<PlayerReference>(true)[boardEntity];

        Entities.WithoutBurst().WithNone<CatTag>().ForEach((Entity e, in HittingGoal goalInfo) =>
        {
            var playerEntity = playerReference[goalInfo.PlayerIndex].Player;
            var score = EntityManager.GetComponentData<Score>(playerEntity);
            ++score.Value;
            EntityManager.SetComponentData(playerEntity, score);
            ecb.DestroyEntity(e);
        }).Run();

        Entities.WithAll<CatTag>().WithAll<HittingGoal>().ForEach((Entity e) =>
        {
            ecb.DestroyEntity(e);
        }).Run();
    }
}
