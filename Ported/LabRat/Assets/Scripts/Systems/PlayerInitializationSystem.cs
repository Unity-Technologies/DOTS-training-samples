using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerInitializationSystem : SystemBase
{
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

        var playerCount = 0;
        
        Entities.WithName("InitPlayers")
            .WithAll<GameStateInitialize>()
            .ForEach((in PlayerInitialization playerInitialization) =>
        {
            playerCount = playerInitialization.PlayerCount;
            for (int i = 0; i < playerInitialization.PlayerCount; i++)
            {
                var playerEntity = ecb.Instantiate(playerInitialization.PlayerPrefab);
                ecb.AddComponent(playerEntity, new PlayerIndex {Value = i});
                
                if (i == 0 && !playerInitialization.AIOnly)
                {
                    ecb.AddComponent<HumanPlayerTag>(playerEntity);

                    var playerArrowPreviewEntity = ecb.Instantiate(playerInitialization.HumanPlayerArrowPreview);
                    ecb.AddComponent<HumanPlayerTag>(playerArrowPreviewEntity);

                    ecb.SetComponent(playerEntity, new Name { Value = "Player" });
                }
                else
                {
                    ecb.AddComponent(playerEntity, new AIPlayerLastDecision { Value = ticks });
                    ecb.SetComponent(playerEntity, new Name { Value = $"Computer {i}" });
                }

                var color = UnityEngine.Color.HSVToRGB(i / (float) playerCount, 1, 1);
                var colorAsFloat4 = new float4(color.r, color.g, color.b, color.a);
                ecb.AddComponent(playerEntity, new LabRat_Color() { Value = colorAsFloat4 });
                ecb.AddBuffer<PlayerArrow>(playerEntity);
            }
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
      
        if (playerCount > 0)
        {
            if (Players.IsCreated)
                Players.Dispose();

            var localP = Players = new NativeArray<Entity>(playerCount, Allocator.Persistent);
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
    }
}
