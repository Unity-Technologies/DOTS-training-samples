using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial class HiveSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var random = new Random(1234);

            Entities
                .WithoutBurst()
                .ForEach((Entity entity, in HiveSpawner spawner, in SpawnData spawnData, in Translation worldCenter, in NonUniformScale worldSize ) =>
                {
                    //PlayField.size = worldSize.Value;
                    //PlayField.origin = worldCenter.Value;

                    float3 resourceSpawnAreaSize = new float3(worldSize.Value.x * 0.5f, worldSize.Value.y, worldSize.Value.z);

                    ecb.RemoveComponent<HiveSpawner>(entity);
                    var worldStartPosition =(worldCenter.Value - worldSize.Value/2);
                    var hiveSize = new float3(0.1f * worldSize.Value.x, worldSize.Value.y, worldSize.Value.z);
                    for (int i = 0; i < spawner.BeesAmount; ++i)
                    {
                        int team = i % 2;

                        var randPos = hiveSize * random.NextFloat3(1);
                        var translation = team == 0
                            ? worldStartPosition + randPos
                            : worldStartPosition + worldSize.Value - randPos;

                        Instantiation.Bee.Instantiate(ecb, spawnData.BeePrefab, translation, team, ref random);
                    }

                    var resourceWorldStartPosition = (worldCenter.Value - resourceSpawnAreaSize / 2);
                    for (int i = 0; i < spawner.ResourceAmount; ++i)
                    {
                        var resource = ecb.Instantiate(spawner.ResourcePrefab);
                        var translation = new Translation { Value = resourceWorldStartPosition + resourceSpawnAreaSize * random.NextFloat3(1) };
                        translation.Value.y = -resourceSpawnAreaSize.y / 2;
                        ecb.SetComponent(resource, translation);
                        // var body = new KinematicBody()
                        //     { LandPosition = new float3(translation.Value.x, -resourceSpawnAreaSize.y / 2, translation.Value.z) };
                        // ecb.SetComponent(resource, body);
                        // ecb.RemoveComponent<KinematicBody>(resource);
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}