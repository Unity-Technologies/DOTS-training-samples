using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class DustSpawnSystem : SystemBase
{
    private static readonly int HivePosition = Shader.PropertyToID("_HivePosition");

    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfiguration>();
        var commandBuffer = endSim.CreateCommandBuffer();
        var random = new Unity.Mathematics.Random((uint)(Time.ElapsedTime * 10000)+1);
        
        Entities
            .ForEach((Entity entity, in DustSpawnConfiguration configuration, in Translation translation) =>
            {
                for (int i = 0; i < configuration.Count; i++)
                {
                    var dustEntity = commandBuffer.Instantiate(gameConfig.DustPrefab);
                    commandBuffer.SetComponent(dustEntity, translation);

                    var variation = random.NextFloat3Direction();
                    commandBuffer.SetComponent(dustEntity, new Velocity() { Value = (configuration.Direction * variation) * 5f });
                }
                
                commandBuffer.RemoveComponent<DustSpawnConfiguration>(entity);
            }).Schedule();
        
        endSim.AddJobHandleForProducer(Dependency);
    }
}