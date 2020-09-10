using Unity.Collections;
using Unity.Entities;

using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerInitializationSystem : SystemBase
{
    private EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ticks = System.DateTime.Now.Ticks;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities.WithName("InitPlayers")
            .WithAll<GameStateInitialize>()
            .ForEach((in PlayerInitialization playerInitialization) =>
        {
            var playerCount = playerInitialization.PlayerCount;
            for (int i = 0; i < playerInitialization.PlayerCount; i++)
            {
                var playerEntity = ecb.Instantiate(playerInitialization.PlayerPrefab);
                if (i == 0 && !playerInitialization.AIOnly)
                {
                    ecb.AddComponent<HumanPlayerTag>(playerEntity);

                    var playerArrowPreviewEntity = ecb.Instantiate(playerInitialization.HumanPlayerArrowPreview);
                    ecb.AddComponent<HumanPlayerTag>(playerArrowPreviewEntity);

                    ecb.SetComponent(playerEntity, new Name { Value = "You" });
                }
                else
                {
                    ecb.AddComponent(playerEntity, new AIPlayerLastDecision { Value = ticks });
                    ecb.SetComponent(playerEntity, new Name { Value = $"Computer {i}" });
                }
                ecb.SetComponent(playerEntity, new ColorAuthoring() { Color = UnityEngine.Color.HSVToRGB(i / (float)playerCount, 1, 1) });
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}