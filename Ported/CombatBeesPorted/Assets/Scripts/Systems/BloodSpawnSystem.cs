using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BloodSpawnSystem: SystemBase
{
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
            .ForEach((Entity entity, in BloodSpawnConfiguration configuration, in Translation translation) =>
            {
                for (int i = 0; i < configuration.Count; i++)
                {
                    var bloodEntity = commandBuffer.Instantiate(gameConfig.BloodDropletPrefab);
                    commandBuffer.SetComponent(bloodEntity, translation);

                    var variation = random.NextFloat3(configuration.Direction) * 2f;
                    commandBuffer.SetComponent(bloodEntity, new Force() { Value = (configuration.Direction + variation) * 100f });
                    commandBuffer.AddComponent<ShrinkAndDestroy>(bloodEntity, new ShrinkAndDestroy() { lifetime = 4f, age = -2f });
                }
                
                commandBuffer.RemoveComponent<BloodSpawnConfiguration>(entity);
            }).Schedule();
        
        endSim.AddJobHandleForProducer(Dependency);
    }
}