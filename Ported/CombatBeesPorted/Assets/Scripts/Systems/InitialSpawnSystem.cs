
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class InitialSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfiguration>();
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random((uint)(Time.ElapsedTime * 10000)+1);

        var distance = 10f;

        Entities
            .ForEach((in Entity entity, in InitialSpawnConfiguration config) =>
            {
                for (int i = 0; i < config.FoodCount; i++)
                {
                    var foodEntity = commandBuffer.Instantiate(gameConfig.FoodPrefab);
                    commandBuffer.SetComponent(foodEntity, new Translation() { Value = (random.NextFloat3Direction() * distance * new float3(1, 0, 1)) + new float3(0,.25f,0)});
                    commandBuffer.AddComponent(foodEntity, new Force() {});
                    commandBuffer.AddComponent(foodEntity, new Velocity() {});
                }

                var TeamABeeSpawner=commandBuffer.CreateEntity();
                commandBuffer.AddComponent(TeamABeeSpawner,new BeeSpawnConfiguration(){Count = config.FoodCount});
                commandBuffer.AddComponent(TeamABeeSpawner,new TeamA());
                commandBuffer.AddComponent(TeamABeeSpawner,new Translation(){Value = new float3(-gameConfig.HivePosition,0,0) });
                var TeamBBeeSpawner=commandBuffer.CreateEntity();
                commandBuffer.AddComponent(TeamBBeeSpawner,new BeeSpawnConfiguration(){Count = config.FoodCount});
                commandBuffer.AddComponent(TeamBBeeSpawner,new TeamB());
                commandBuffer.AddComponent(TeamBBeeSpawner,new Translation(){Value = new float3(gameConfig.HivePosition,0,0) });
                
                commandBuffer.DestroyEntity(entity);
            }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}