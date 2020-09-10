using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInitializationSystem : SystemBase
{
    EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities.ForEach((int entityInQueryIndex, in Entity playerInitializationEntity, in PlayerInitialization playerInitialization) => {
            var playerCount = playerInitialization.PlayerCount;
            for (int i = 0; i < playerInitialization.PlayerCount; i++)
            {
                var playerEntity = ecb.Instantiate(entityInQueryIndex, playerInitialization.PlayerPrefab);
                if (i == 0 && !playerInitialization.AIOnly)
                {
                    ecb.AddComponent<HumanPlayerTag>(entityInQueryIndex, playerEntity);

                    var playerArrowPreviewEntity = ecb.Instantiate(entityInQueryIndex, playerInitialization.HumanPlayerArrowPreview);
                    ecb.AddComponent<HumanPlayerTag>(entityInQueryIndex, playerArrowPreviewEntity);

                    ecb.SetComponent(entityInQueryIndex, playerEntity, new Name { Value = "You" });
                }
                else
                {
                    ecb.SetComponent(entityInQueryIndex, playerEntity, new Name { Value = $"Computer {i}" });
                    ecb.SetComponent(entityInQueryIndex, playerEntity, new Color { Value = (Vector4)UnityEngine.Color.HSVToRGB(i / (float)playerCount, 1, 1) });
                }

            }
            ecb.AddComponent<Disabled>(entityInQueryIndex, playerInitializationEntity);    
        }).ScheduleParallel();
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
