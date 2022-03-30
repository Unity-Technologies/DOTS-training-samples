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

    protected override void OnCreate()
    {
        // Run ONLY if PlatformSpawnerComponent exists
        RequireForUpdate(spawnerQuery);
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

        Entities.ForEach((Entity Entity, in DynamicBuffer<LineMarkerBufferElement> lineMarker) =>
        {
            for (int i = 0; i < lineMarker.Length; i++)
            {
                if (lineMarker[i].IsPlatform && i < lineMarker.Length - 2)
                {
                    var instance = ecb.Instantiate(platformPrefab);

                    var translation = new Translation();
                    translation.Value.x = lineMarker[i].Position.x + (lineMarker[i + 1].Position.x - lineMarker[i].Position.x) / 2;
                    translation.Value.y = lineMarker[i].Position.y + (lineMarker[i + 1].Position.y - lineMarker[i].Position.y) / 2;
                    translation.Value.z = lineMarker[i].Position.z + (lineMarker[i + 1].Position.z - lineMarker[i].Position.z) / 2;
                    ecb.SetComponent(instance, translation);

                    var rotation = new Rotation();
                    rotation.Value = Quaternion.LookRotation(lineMarker[i + 1].Position - lineMarker[i].Position, Vector3.up); //calc a rotation that
                    rotation.Value *= Quaternion.Euler(0, 90, 0);
                    ecb.SetComponent(instance, rotation);
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
