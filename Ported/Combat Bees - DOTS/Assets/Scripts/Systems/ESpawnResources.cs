using Unity.Collections;
using Unity.Entities;

public partial class SpawnResources : SystemBase
{
    private int j = 0;
    
    protected override void OnCreate()
    {
        //RequireSingletonForUpdate<ESingeltonHybridSpawner>();
    }

    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Entities
        //     .ForEach((Entity entity, ref EResourceComponent resourceComponent) =>
        //     {
        //
        //         for (int i = 0; i < resourceComponent.startResourceCount; i++)
        //         {
        //             var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
        //             var translation = new Translation();
        //             translation.Value = new float3(
        //                 resourceComponent.random.NextInt(-20, 20) * resourceComponent.gridX,
        //                 20,
        //                 resourceComponent.random.NextInt(-5, 5)) * resourceComponent.gridZ;
        //             ecb.SetComponent(instance, translation);
        //             ecb.RemoveComponent<EResourceComponent>(entity);
        //             resourceComponent.spawnedResources += 1;
        //             //return resourceComponent.spawnedResources;
        //         }
        //     }).WithoutBurst().Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }   
}