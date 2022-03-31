using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PassengerSpawnerSystem : SystemBase
{
    private EntityQuery spawnerQuery;

    protected override void OnCreate()
    {
        // Run ONLY if PassengerSpawnerComponent exists
        RequireForUpdate(spawnerQuery);
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

        Entities.ForEach((Entity entity, int entityInQueryIndex, in DynamicBuffer<LineMarkerBufferElement> lineMarker) =>
        {
            var random = new Random((uint)entityInQueryIndex+1);
            for (int i = 0; i < lineMarker.Length; i++)
            {
                if (lineMarker[i].IsPlatform)
                {
                    for (int j = 0; j < numPassengersToSpawn; j++)
                    {
                        var instance = ecb.Instantiate(passengerPrefab);

                        var translation = new Translation();
                        translation.Value = lineMarker[i].Position;
                        ecb.SetComponent(instance, translation);

                        ecb.SetComponent(instance, new Passenger
                        {
                            // TODO: Randomize destination for passengers.
                            FinalDestination = float3.zero,
                            WalkSpeed = random.NextFloat(0.01f, 0.10f)
                        });
                    }
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
