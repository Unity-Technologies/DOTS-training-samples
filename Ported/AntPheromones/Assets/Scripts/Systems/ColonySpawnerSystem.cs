using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ColonySpawnerSystem : SystemBase
{
	const int mapSize = 128;
    protected override void OnUpdate()
    {
        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
	        Entities
		        .ForEach((Entity entity, in ColonySpawner spawner) =>
                {
                    ecb.DestroyEntity(entity);

                    var colonyEntity = ecb.Instantiate(spawner.ColonyPrefab);
                    ecb.SetComponent(colonyEntity, new Translation
                    {
                        Value = new float3(mapSize/2f,mapSize/2f,0f)
                    });
                    ecb.AddComponent(colonyEntity, new Colony());
                }).Run();


            ecb.Playback(EntityManager);
        }
    }
}
