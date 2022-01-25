using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SetupGameSystem))]
public partial class PropagateFireSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var gameConstants = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer();
        float deltaTime = Time.DeltaTime;
        
        Entities.ForEach((Entity e, ref FireField fireField) =>
        {
            var field = GetBuffer<FireHeat>(e);

            fireField.TimeUntilNextUpdate -= deltaTime;
            if (fireField.TimeUntilNextUpdate > 0)
            {
                return;
            }

            fireField.TimeUntilNextUpdate += gameConstants.FireSimUpdateRate;
            for (int outerY = 0; outerY < gameConstants.FieldSize.y; outerY++)
            {
                for (int outerX = 0; outerX < gameConstants.FieldSize.x; outerX++)
                {
                    float temperature = field[outerX + outerY * gameConstants.FieldSize.x].Value;
                    float temperatureChange = 0f;
                    
                    for (int x = math.max(outerX - gameConstants.FireHeatTransferRadius, 0); x <= math.min(outerX + gameConstants.FireHeatTransferRadius, gameConstants.FieldSize.x - 1); x++)
                    {
                        for (int y = math.max(outerY - gameConstants.FireHeatTransferRadius, 0);
                             y <= math.min(outerY + gameConstants.FireHeatTransferRadius, gameConstants.FieldSize.y - 1);
                             y++)
                        {
                            if (field[x + y * gameConstants.FieldSize.x] < 0.2f)
                                continue;

                            temperatureChange += field[x + y * gameConstants.FieldSize.x] * gameConstants.FireHeatTransferRate;
                        }
                    }

                    field[outerX + outerY * gameConstants.FieldSize.x] =
                        math.clamp(temperature + temperatureChange, 0f, 1f);
                    
                    if (temperature  < gameConstants.FireHeatFlashPoint && temperature + temperatureChange > gameConstants.FireHeatFlashPoint)
                    {
                        var flameEntity = ecb.Instantiate(gameConstants.FlamePrefab);
                        ecb.SetComponent(flameEntity, new Translation { Value = new float3(outerX, 0, outerY) * 0.3f });
                    }
                }
            }
        }).Schedule();
        
        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
