using Unity.Collections;
using Unity.Entities;

public class ResetSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }
    
    protected override void OnUpdate()
    {
        GameConfigComponent config = GetSingleton<GameConfigComponent>();

        if (!UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
            return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithAll<WaterTagComponent>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        Entities.WithAll<FlameCellTagComponent>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        Entities.WithAll<HeatMapElement>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        Entities.WithAll<BucketActiveComponent>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        Entities.WithAll<BotsChainComponent>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        Entities.WithAll<TargetBucket>().ForEach((Entity ent) =>
        {
            ecb.DestroyEntity(ent);
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();

        World.GetExistingSystem<BotsChainSpawnSystem>().Enabled = true;
        World.GetExistingSystem<BucketSpawnSystem>().Enabled = true;
        World.GetExistingSystem<FlameCellSpawnSystem>().Enabled = true;
        World.GetExistingSystem<WaterSpawnSystem>().Enabled = true;
    }
}
