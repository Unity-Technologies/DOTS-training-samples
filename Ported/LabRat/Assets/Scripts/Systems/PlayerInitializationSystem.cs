using Unity.Collections;
using Unity.Entities;

using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerInitializationSystem : SystemBase
{
    EntityQuery m_AnyPlayer;

    protected override void OnCreate()
    {
        m_AnyPlayer = GetEntityQuery(ComponentType.ReadOnly<Player>());
    }

    public NativeArray<Entity> Players;

    protected override void OnDestroy()
    {
        if (Players.IsCreated)
            Players.Dispose();
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
                ecb.AddBuffer<PlayerArrow>(playerEntity);
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
      
        if (Players.IsCreated == false)
        {
            var q = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadOnly<Player>()}
            });
            var cc = q.CalculateEntityCount();
            var localP = Players = new NativeArray<Entity>(cc, Allocator.Persistent);
            ecb = new EntityCommandBuffer(Allocator.Temp);
            Entities.WithName("InitPlayerArray")
                .WithNone<PlayerInitializedTag>()
                .ForEach((int entityInQueryIndex, Entity e, in Player p) =>
                {
                    localP[entityInQueryIndex] = e;
                    ecb.AddComponent<PlayerInitializedTag>(e);
                }).Run();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    
        // Quick'n'dirty cleanup
        if (EntityManager.HasComponent<GameStateCleanup>(GetSingletonEntity<PlayerInitialization>()))
            EntityManager.DestroyEntity(m_AnyPlayer);
    }
}
