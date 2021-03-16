using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeSpawnerSystem: SystemBase
{
    protected override void OnUpdate()
    {

        var gameConfig = GetSingleton<GameConfiguration>();
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random((uint)(Time.DeltaTime * 10000)+1);
        var speed = 2f;
        
        Entities
            .WithAll<TeamA>()
            .ForEach((in Entity entity, in BeeSpawnConfiguration configuration, in Translation translation) =>
            {
                for (int i = 0; i < configuration.Count; i++)
                {
                    var beeEntity = commandBuffer.Instantiate(gameConfig.BeeTeamAPrefab);
                    commandBuffer.SetComponent(beeEntity, translation);
                    commandBuffer.SetComponent(beeEntity, new Velocity() { Value = random.NextFloat3Direction() * speed });
                }
                
                commandBuffer.DestroyEntity(entity);
            }).Run();
        
        Entities
            .WithAll<TeamB>()
            .ForEach((in Entity entity, in BeeSpawnConfiguration configuration, in Translation translation) =>
            {
                for (int i = 0; i < configuration.Count; i++)
                {
                    var beeEntity = commandBuffer.Instantiate(gameConfig.BeeTeamBPrefab);
                    commandBuffer.SetComponent(beeEntity, translation);
                    commandBuffer.SetComponent(beeEntity, new Velocity() { Value = random.NextFloat3Direction() * speed });
                }
                
                commandBuffer.DestroyEntity(entity);
            }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
