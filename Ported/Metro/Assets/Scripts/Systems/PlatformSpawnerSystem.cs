using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlatformSpawnerSystem : SystemBase
{
    private EntityQuery spawnerQuery;
    private EntityQuery stationQuery;

    protected override void OnCreate()
    {
        // Run ONLY if PlatformSpawnerComponent exists
        RequireForUpdate(spawnerQuery);
        RequireForUpdate(stationQuery);
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entity platformPrefab = new Entity();
        float3 position = 0;

        Entities
            .WithStoreEntityQueryInField(ref spawnerQuery)
            .ForEach((Entity entity, in PlatformSpawnerComponent platformSpawner) =>
            {
                platformPrefab = platformSpawner.PlatformPrefab;
                position = platformSpawner.Position;

                ecb.DestroyEntity(entity);
            }).Run();

        Entities.WithStoreEntityQueryInField(ref stationQuery)
            .ForEach((Entity Entity, in StationComponent stationComponent, in Translation translation,  
            in Rotation rotation) =>
        {
            var instance = ecb.Instantiate(platformPrefab);
            ecb.SetComponent(instance, new Translation{Value = translation.Value});
            ecb.SetComponent(instance, new Rotation{Value = rotation.Value});
            
            /*for (int i = 0; i < lineMarker.Length; i++)
            {
                if (lineMarker[i].IsPlatform && i < lineMarker.Length - 1)
                {
                    var instance = ecb.Instantiate(platformPrefab);

                    var platformTranslation = new Translation();
                    platformTranslation.Value = translation.Value;
                    ecb.SetComponent(instance, platformTranslation);

                    var rotation = new Rotation();
                    rotation.Value = Quaternion.LookRotation(lineMarker[i + 1].Position - lineMarker[i].Position, Vector3.up); //calc a rotation that
                    rotation.Value *= Quaternion.Euler(0, 90, 0);
                    ecb.SetComponent(instance, rotation);
                    
                    ecb.AddComponent<PlatformComponent>(instance);
                }
            }*/
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
