using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

class ResourceSimulationSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer( Allocator.TempJob );
        var parallelECB = ecb.AsParallelWriter();
        var gameConfig = GetSingleton<GameConfig>();
        var fieldConfig = GetSingleton<ShaderOverrideCenterSize>();

        float deltaTime = Time.DeltaTime;
        var gravity = gameConfig.Gravity;
        var fieldWidth = gameConfig.PlayingFieldSize.x;
        var baseSize = (1 - fieldConfig.Value) * fieldWidth / 2;
        
        Entities
            .ForEach((Entity entity, ref Resource resource, ref NewTranslation pos) =>
            {
                if(HasComponent<Translation>(resource.CarryingBee))
                {
                    var beePos = GetComponent<Translation>(resource.CarryingBee);
                    pos.translation.Value = beePos.Value - new float3(0.0f, 0.01f, 0.0f);
                }
                else
                    if(pos.translation.Value.y>0.0f)
                    {
                        pos.translation.Value.y = math.max(0.0f, pos.translation.Value.y + resource.Speed * deltaTime);
                        resource.Speed -= gravity * deltaTime;
                    }
                    else
                    {
                        // hit the ground. check if inside base

                         var xmod = math.abs(pos.translation.Value.x);
                         if ((fieldWidth / 2 - baseSize - xmod) < 0f)
                         {
                             parallelECB.DestroyEntity(0, entity);
                             // spawn new bees where it hit the ground
                             var beePrefab = gameConfig.BeePrefab;
                             var spawnerConfig = new SpawnBeeConfig
                             {
                                 BeeCount = gameConfig.BeeSpawnCountOnResourceDrop,
                                 BeePrefab = beePrefab,
                                 SpawnAreaSize = new float3(0.01f, 0.01f, 0.01f),
                                 SpawnLocation = pos.translation.Value,
                                 Team = pos.translation.Value.x < 0f ? 0 : 1
                             };
                             var beeSpawner = parallelECB.CreateEntity(0);
                             parallelECB.AddComponent(0, beeSpawner, spawnerConfig);
                         }
                    }
            }).ScheduleParallel();
        Dependency.Complete();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
