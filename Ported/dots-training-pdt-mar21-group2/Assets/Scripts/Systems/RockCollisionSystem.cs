using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class RockCollisionSystem : SystemBase
{
    static Random s_Random = new Random(0x1234);
    private EntityQuery m_AllCans;

    protected override void OnCreate()
    {
        base.OnCreate();
        var desc = new EntityQueryDesc();
        desc.All = new ComponentType[]
        {
            ComponentType.ReadOnly(typeof(Can)), ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Velocity)), ComponentType.ReadOnly(typeof(AngularVelocity))
        };
        desc.None = new ComponentType[] {ComponentType.ReadOnly(typeof(Falling))};

        m_AllCans = EntityManager.CreateEntityQuery(desc);
    }

    protected override void OnUpdate()
    {
        var canEntities = m_AllCans.ToEntityArray(Allocator.TempJob);
        var canTranslations = m_AllCans.ToComponentDataArray<Translation>(Allocator.TempJob);

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var ecbParaWriter = ecb.AsParallelWriter();

        var random = s_Random;
        var seed = random.NextUInt();

        // Only apply the collision check on falling rocks (has been thrown)
        Entities.WithAll<Falling, Rock>()
            .WithReadOnly(canEntities)
            .WithDisposeOnCompletion(canEntities)
            .WithReadOnly(canTranslations)
            .WithDisposeOnCompletion(canTranslations)
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity rockVelocity, in Translation rockTranslation,
                    in Scale rockScale) =>
                RockCansCollision(ref ecbParaWriter, entityInQueryIndex, ref random, seed, rockTranslation, rockScale,
                    ref rockVelocity, canEntities, canTranslations))
            .ScheduleParallel();

        sys.AddJobHandleForProducer(Dependency);
        s_Random = random;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IntersectSphere(float3 p0, float r0, float3 p1, float r1)
    {
        float threshold = r0 + r1;
        return math.distancesq(p0, p1) < threshold * threshold;
    }

    static void RockCansCollision(ref EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, ref Random random,
        uint seed, in Translation rockTranslation, in Scale rockScale, ref Velocity rockVelocity,
        in NativeArray<Entity> canEntities, in NativeArray<Translation> canTranslations)
    {
        random.InitState(seed * ((uint) entityInQueryIndex + 0x12345678));

        // Demo is not physically accurate, collision resolution is done by assigning random velocities

        // Rock has a radius of 0.5
        // Can has a height of 2 and radius of 0.5 -> a sphere of radius 1 can cover the whole can
        const float rockRadius = 0.5f;
        const float canRadius = 1.0f;

        Assert.AreEqual(canEntities.Length, canTranslations.Length);

        bool rockHit = false;

        for (int i = 0; i < canTranslations.Length; ++i)
        {
            const float canScale = 1.0f; // Ignore the scale of cans spawning in
            if (IntersectSphere(rockTranslation.Value, rockRadius * rockScale.Value, canTranslations[i].Value,
                canRadius * canScale))
            {
                rockHit = true;
                ecb.RemoveComponent<Available>(entityInQueryIndex, canEntities[i]);
                ecb.AddComponent<Falling>(entityInQueryIndex, canEntities[i]);
                ecb.SetComponent(entityInQueryIndex, canEntities[i], new Velocity {Value = rockVelocity.Value});
                ecb.SetComponent(entityInQueryIndex, canEntities[i], new AngularVelocity
                    {Value = math.length(rockVelocity.Value) * random.NextFloat3(-20.0f, 20.0f)});
            }
        }

        if (rockHit)
        {
            rockVelocity.Value = random.NextFloat3(1.5f, 1.5f);
        }
    }
}