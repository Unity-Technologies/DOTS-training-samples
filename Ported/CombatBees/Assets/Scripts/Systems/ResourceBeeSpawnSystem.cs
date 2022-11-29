using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
//using UnityEngine;

[BurstCompile]
partial struct ResourceBeeSpawnSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var beeSizeHalfRange = (config.maximumBeeSize - config.minimumBeeSize) * .5f;
        var beeSizeMiddle = config.minimumBeeSize + beeSizeHalfRange;
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (hive, team) in SystemAPI.Query<RefRO<Hive>, Team>())
        {
            var hiveVal = hive.ValueRO;
            var hiveBottom = hiveVal.boundsPosition.y - hiveVal.boundsExtents.y;
            var hiveLeft = hiveVal.boundsPosition.x - hiveVal.boundsExtents.x;
            var hiveRight = hiveVal.boundsPosition.x + hiveVal.boundsExtents.x;
            foreach (var (resource, trans, entity) in SystemAPI.Query<RefRO<Resource>, RefRO<LocalTransform>>().WithAny<ResourceDropped>().WithEntityAccess())
            {
                var resourcePosition = trans.ValueRO.Position;
                var resourceBottom = trans.ValueRO.Position.y - resource.ValueRO.boundsExtents.y;
                if (resourceBottom <= hiveBottom && resourcePosition.x >= hiveLeft && resourcePosition.x <= hiveRight)
                {
                    SpawnBees(config, ecb, team, hive, beeSizeHalfRange, beeSizeMiddle, trans);
                    ecb.DestroyEntity(entity);
                    SpawnParticles(config, ecb, trans.ValueRO.Position, 5);
                }
            }
        }
    }

    private static void SpawnParticles(Config config, EntityCommandBuffer ecb, float3 position, int count)
    {
        var particles = new NativeArray<Entity>(count, Allocator.Temp);
        ecb.Instantiate(config.particlePrefab, particles);
        var color = new URPMaterialPropertyBaseColor { Value = new float4(1f, 1f, 1f, 1f) };
        var random = new Random();
        random.InitState((uint)position.GetHashCode());
        foreach (var particle in particles)
        {
            ecb.SetComponent(particle, color);
            ecb.SetComponent(particle, new LocalTransform
            {
                Position = position,
                Scale = random.NextFloat(.25f, .5f),
                Rotation = quaternion.identity
            });
            ecb.SetComponent(particle, new Particle()
            {
                lifeTime = random.NextFloat(.25f, .5f),
                velocity = random.NextFloat3Direction() * 5f
            });
        }
    }

    private static void SpawnBees(Config config, EntityCommandBuffer ecb, Team team, RefRO<Hive> hive, float beeSizeHalfRange, float beeSizeMiddle, RefRO<LocalTransform> trans)
    {
        var bees = new NativeArray<Entity>(config.beesPerResource, Allocator.Temp);
        ecb.Instantiate(config.beePrefab, bees);
        ecb.SetSharedComponent(bees, new Team()
        {
            number = team.number
        });
        var hiveValue = hive.ValueRO;
        var color = new URPMaterialPropertyBaseColor { Value = hiveValue.color };

        foreach (var bee in bees)
        {
            ecb.SetComponent(bee, color);
            var pos = trans.ValueRO.Position;
            pos.y = bee.Index;
            var scaleRandom = math.clamp(noise.cnoise(pos / 13f) * 2f, -1f, 1f);
            var scaleDelta = scaleRandom * beeSizeHalfRange;
            var scale = math.clamp(scaleDelta + beeSizeMiddle,
                config.minimumBeeSize, config.maximumBeeSize);
            ecb.SetComponent(bee, new LocalTransform
            {
                Position = trans.ValueRO.Position,
                Scale = scale
            });
        }
    }
}