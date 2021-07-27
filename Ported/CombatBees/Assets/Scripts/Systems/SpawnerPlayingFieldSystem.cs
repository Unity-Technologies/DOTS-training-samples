using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
class SpawnerPlayingFieldSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in SpawnPlayingFieldConfig spawner) =>
            {
                ecb.DestroyEntity(entity);
                var instance = ecb.Instantiate(spawner.PlayingFieldPrefab);
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
