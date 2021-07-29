using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
class SpawnerPlayingFieldSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var gameConfig = GetSingleton<GameConfig>();
        var playingFieldSize = gameConfig.PlayingFieldSize;

        Entities
            .ForEach((Entity entity, in SpawnPlayingFieldConfig spawner) =>
            {
                ecb.DestroyEntity(entity);
                var field = ecb.Instantiate(spawner.PlayingFieldPrefab);
                ecb.SetComponent(field, new NonUniformScale()
                    { Value = playingFieldSize });
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
