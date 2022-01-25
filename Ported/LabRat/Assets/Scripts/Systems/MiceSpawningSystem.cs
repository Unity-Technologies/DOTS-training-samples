using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateBefore(typeof(CreatureMover))]
public partial class MiceSpawningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var deltaTime = Time.DeltaTime;
        var configEntity = GetSingletonEntity<Config>();
        var config = GetComponent<Config>(configEntity);
        
        Entities
            .ForEach((ref MiceSpawner spawner, ref Tile tile, ref Direction direction) =>
            {
                // if we are under a cooldown, tick it down
                if (spawner.SpawnCooldown > 0f)
                {
                    spawner.SpawnCooldown -= deltaTime;
                    // if the cooldown is finally over, top up the amount of mice to spawn
                    if (spawner.SpawnCooldown <= 0f)
                    {
                        spawner.RemainingMiceToSpawn = config.MiceToSpawnPerSpawner;
                    }
                }
                // if our spawn count is over, trigger a cooldown
                else if (spawner.RemainingMiceToSpawn <= 0)
                {
                    var random = new Random(spawner.RandomizerState);
                    spawner.SpawnCooldown +=
                        random.NextFloat(config.MouseSpawnCooldown.x, config.MouseSpawnCooldown.y);
                    spawner.RandomizerState = random.state;
                }
                // spawn new mice
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
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
