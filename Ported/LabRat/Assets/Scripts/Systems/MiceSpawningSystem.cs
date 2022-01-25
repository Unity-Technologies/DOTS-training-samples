using Unity.Collections;
using Unity.Entities;

public partial class MiceSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;
        var configEntity = GetSingletonEntity<Config>();
        var config = GetComponent<Config>(configEntity);
        
        Entities
            .ForEach((Entity entity, ref MiceSpawner spawner, ref Tile tile, ref Direction direction) =>
            {
                // if our spawn count is over, destroy this spawner
                if (spawner.RemainingMiceToSpawn <= 0)
                {
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    // update the counter
                    spawner.SpawnCounter -= deltaTime;
                    if (spawner.SpawnCounter < 0f)
                    {
                        // counter expired, let's spawn a mouse!
                        spawner.SpawnCounter += config.MouseSpawnRate;
                        spawner.RemainingMiceToSpawn -= 1;
                        var mouse = ecb.Instantiate(config.MousePrefab);
                        ecb.SetComponent(mouse, new Direction
                        {
                            Value = direction.Value
                        });
                        ecb.SetComponent(mouse, new Tile
                        {
                            Coords = tile.Coords
                        });
                    }
                }
            }).Schedule();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
