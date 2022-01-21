using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public partial class RandomVelocitySpawner : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref RandomVelocitySpawnerData spawner) =>
            {
                for (int i = 0; i < spawner.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(spawner.PrefabToSpawn);
                    var translation = new Translation();

                    translation.Value = spawner.Position;
                    
                    float3 velocity = spawner.Random.NextFloat3(
                        new float3(-1,-1,-1) * spawner.MaxInitVelocity,
                        new float3(1,1,1) * spawner.MaxInitVelocity);
                    
                    ecb.SetComponent(prefabInstance, translation);
                    ecb.SetComponent(prefabInstance, new Velocity{Value = velocity});
                    ecb.RemoveComponent<RandomVelocitySpawnerData>(entity); // Prevents repeated spawning
                }
            }).WithoutBurst().Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
