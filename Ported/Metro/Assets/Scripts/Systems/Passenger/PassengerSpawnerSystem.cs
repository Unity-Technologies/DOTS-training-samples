using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PassengerSpawnerSystem : SystemBase
{
    private EntityQuery spawnerQuery;
    private EntityQuery platformsQuery;

    protected override void OnCreate()
    {
        // Run ONLY if PassengerSpawnerComponent exists
        RequireForUpdate(spawnerQuery);
        RequireForUpdate(platformsQuery);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entity passengerPrefab = new Entity();
        int numPassengersToSpawn = 0;
        float3 position = 0;

        Entities
            .WithStoreEntityQueryInField(ref spawnerQuery)
            .ForEach((Entity entity, in PassengerSpawnerComponent passengerSpawner) =>
            {
                passengerPrefab = passengerSpawner.PassengerPrefab;
                numPassengersToSpawn = passengerSpawner.PassengersPerStation;
                ecb.DestroyEntity(entity);
            }).Run();

        Entities
            .WithStoreEntityQueryInField(ref platformsQuery)
            .ForEach((Entity entity, int entityInQueryIndex, in PlatformComponent platform, in Translation translation) =>
        {
            var random = new Random((uint)entityInQueryIndex+1);
            
            for (int j = 0; j < numPassengersToSpawn; j++)
            {
                var instance = ecb.Instantiate(passengerPrefab);

                var passengerTranslation = new Translation();
                passengerTranslation.Value = translation.Value;
                ecb.SetComponent(instance, passengerTranslation);

                ecb.SetComponent(instance, new Passenger
                {
                    // TODO: Randomize destination for passengers.
                    CurrentPlatform = entity,
                    CurrentPlatformPosition = translation.Value,
                    WalkSpeed = random.NextFloat(0.01f, 0.10f)
                });
            }
            
            
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
