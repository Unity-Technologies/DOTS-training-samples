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
            var blueColor = new float4(0, 0, 1, 1);
            var yellowColor = new float4(1, 1, 0, 1);

            Entities
                .ForEach((Entity entity, in HiveSpawner spawner, in Translation worldCenter, in NonUniformScale worldSize ) =>
                {
                    PlayField.size = worldSize.Value;
                    PlayField.origin = worldCenter.Value;
                    
                    ecb.RemoveComponent<HiveSpawner>(entity);
                    var worldStartPosition =(worldCenter.Value - worldSize.Value/2);
                    var hiveSize = new float3(0.1f * worldSize.Value.x, worldSize.Value.y, worldSize.Value.z);
                    for (int i = 0; i < spawner.BeesAmount; ++i)
                    {
                        int team = i % 2;
                        var bee = ecb.Instantiate(spawner.BeePrefab);
                        var randPos = hiveSize * random.NextFloat3(1);
                        var translation = new Translation { Value = team == 0
                            ? worldStartPosition + randPos
                            : worldStartPosition + worldSize.Value - randPos
                        };
                        ecb.SetComponent(bee, translation);
                        ecb.SetComponent(bee, new URPMaterialPropertyBaseColor
                        {
                            Value = team == 0 ? blueColor : yellowColor
                        });
                        ecb.SetComponent(bee, new BeeMovement
                        {
                            Velocity = float3.zero,
                            Size = random.NextFloat(0.25f, 0.5f)
                        });
                        ecb.AddSharedComponent(bee, new Team { TeamId = team });
                    }
                    
                    for (int i = 0; i < spawner.ResourceAmount; ++i)
                    {
                        var resource = ecb.Instantiate(spawner.ResourcePrefab);
                        var translation = new Translation { Value = worldStartPosition + worldSize.Value * random.NextFloat3(1) };
                        translation.Value.y = -worldSize.Value.y / 2;
                        ecb.SetComponent(resource, translation);
                        // var body = new KinematicBody()
                        //     { LandPosition = new float3(translation.Value.x, -worldSize.Value.y / 2, translation.Value.z) };
                        // ecb.SetComponent(resource, body);
                        ecb.RemoveComponent<KinematicBody>(resource);
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}