using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Random(1234);
        
        Entities.ForEach((Entity entity, in Spawner spawner) =>
        {
            ecb.DestroyEntity(entity);

            for (int i = 0; i < spawner.LaneCount; ++i)
            {
                var instance = ecb.Instantiate(spawner.LanePrefab);
                var translation = new Translation {Value = new float3(0, 0, i)};
                ecb.SetComponent(instance,translation);

                for (int j = 0; j < 100; ++j)
                {
                    if (random.NextFloat() < spawner.CarFrequency)
                    {
                        var vehicle = ecb.Instantiate(spawner.CarPrefab);
                        
                        ecb.SetComponent(vehicle, new Translation
                        {
                            Value = new float3(0,0,i)
                        });
                        
                        ecb.SetComponent(vehicle, new URPMaterialPropertyBaseColor
                        {
                            Value = random.NextFloat4()
                        });
                        
                        ecb.SetComponent(vehicle, new CarMovement
                        {
                            Offset = j
                        });
                    }
                }
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}