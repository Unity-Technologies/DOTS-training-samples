using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public class ParticleSpawningSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, EmitterProperties emitter, ref Spawner spawner, ref LocalToWorld localToWorld) =>
        {
            var particleSpawnCount = spawner.particleCount;

            var spawnPositions = new NativeArray<float3>(particleSpawnCount, Allocator.TempJob);

            GeneratePoints.RandomPointsInUnitSphere(spawnPositions);

            // Calling Instantiate once per spawned Entity is rather slow, and not recommended
            // This code is placeholder until we add the ability to bulk-instantiate many entities from an ECB
            var entities = new NativeArray<Entity>(particleSpawnCount, Allocator.Temp);
            for (int i = 0; i < particleSpawnCount; ++i)
            {
                entities[i] = PostUpdateCommands.Instantiate(spawner.particlePrefab);
            }

            for (int i = 0; i < particleSpawnCount; i++)
            {
                PostUpdateCommands.SetComponent(entities[i], new LocalToWorld
                {
                    Value = float4x4.TRS(
                        localToWorld.Position + (spawnPositions[i] * spawner.spawnRadius),
                        quaternion.LookRotationSafe(spawnPositions[i], math.up()),
                        new float3(1.0f, 1.0f, 1.0f))
                });
                PostUpdateCommands.RemoveComponent<Translation>(entities[i]);
                PostUpdateCommands.RemoveComponent<Rotation>(entities[i]);
                PostUpdateCommands.AddSharedComponent(entities[i], emitter);
            }

            PostUpdateCommands.RemoveComponent<Spawner>(e);

            spawnPositions.Dispose();
            entities.Dispose();
        });
    }
}
