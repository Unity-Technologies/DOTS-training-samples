using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;

[UpdateAfter(typeof(StationSpawnerSystem))]
public partial class PassengerSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var stationsQuery = GetEntityQuery(ComponentType.ReadOnly<Station>(), ComponentType.ReadOnly<TrackID>(), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Rotation>());
        var stations = stationsQuery.ToComponentDataArray<Station>(Allocator.TempJob);
        var stationsTranslations = stationsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var stationsRotations = stationsQuery.ToComponentDataArray<Rotation>(Allocator.TempJob);
        var random = new Random(0xDEADBEEF);
        
        Entities
            .ForEach((Entity entity, in PassengerSpawner spawner) =>
            {
                // Kill our spawner
                ecb.DestroyEntity(entity);

                if (0 == stations.Length)
                    return;
                
                int passengerPerStation = spawner.TotalCount / stations.Length;
                int remainingPassenger = spawner.TotalCount;
                
                for (int s = 0; s < stations.Length; ++s)
                {
                    var spawnPoint = stationsTranslations[s].Value;
                    var rotation = stationsRotations[s].Value;
                    
                    for (int p = 0; p < (s != stations.Length - 1 ? passengerPerStation : remainingPassenger); ++p)
                    {
                        var offset = ( new float3( 3.0f * random.NextFloat() + 1f, 0, 9f * (random.NextFloat()-0.5f)) );
                        var position = spawnPoint + math.mul(rotation, offset);
                        var instance = ecb.Instantiate(spawner.PassengerPrefab);
                        var translation = new Translation {Value = position};
                        ecb.SetComponent(instance, translation);
                        ecb.SetComponent(instance, new Rotation{Value = rotation});
                        ecb.AddComponent(instance, new Passenger());
                    }
                
                    remainingPassenger -= passengerPerStation;
                }
            }).Run();

        stations.Dispose();
        stationsTranslations.Dispose();
        stationsRotations.Dispose();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}