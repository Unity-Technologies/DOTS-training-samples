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
    private EntityQuery m_FallingRocks;
    private NativeList<Entity> m_RockEntities;
    private NativeList<Translation> m_RockTranslations;
    private NativeList<Scale> m_RockScales;
    private NativeList<Velocity> m_RockVelocities;

    protected override void OnCreate()
    {
        var desc = new EntityQueryDesc();
        desc.All = new ComponentType[]
        {
            ComponentType.ReadOnly(typeof(Can)), ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Velocity))
        };
        desc.None = new ComponentType[] {ComponentType.ReadOnly(typeof(Falling))};

        m_AllCans = EntityManager.CreateEntityQuery(desc);
        m_FallingRocks = EntityManager.CreateEntityQuery(ComponentType.ReadOnly(typeof(Rock)),
            ComponentType.ReadOnly(typeof(Falling)));

        var rockCount = 10;
        m_RockEntities = new NativeList<Entity>(rockCount, Allocator.Persistent);
        m_RockTranslations = new NativeList<Translation>(rockCount, Allocator.Persistent);
        m_RockScales = new NativeList<Scale>(rockCount, Allocator.Persistent);
        m_RockVelocities = new NativeList<Velocity>(rockCount, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        m_RockEntities.Dispose();
        m_RockTranslations.Dispose();
        m_RockScales.Dispose();
        m_RockVelocities.Dispose();
    }

    protected override void OnUpdate()
    {
        // Do a first pass of rocks to find those which would be near the can carousel
        var canCarouselAABB = Utils.GetCanCarouselAABB(this);

        var rockCount = m_FallingRocks.CalculateEntityCount();

        m_RockEntities.Length = rockCount;
        m_RockTranslations.Length = rockCount;
        m_RockScales.Length = rockCount;
        m_RockVelocities.Length = rockCount;

        m_RockEntities.Clear();
        m_RockTranslations.Clear();
        m_RockScales.Clear();
        m_RockVelocities.Clear();

        var rockEntityWriter = m_RockEntities.AsParallelWriter();
        var rockTranslationWriter = m_RockTranslations.AsParallelWriter();
        var rockScaleWriter = m_RockScales.AsParallelWriter();
        var rockVelocitiesWriter = m_RockVelocities.AsParallelWriter();

        var cullingJob = Entities
            .WithAll<Falling, Rock>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation t, in Scale s, in Velocity v) =>
                AddRockIfInAABB(ref rockEntityWriter, ref rockTranslationWriter, ref rockScaleWriter,
                    ref rockVelocitiesWriter, entity, entityInQueryIndex, canCarouselAABB, t, s, v))
            .ScheduleParallel(Dependency);

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var ecbParaWriter = ecb.AsParallelWriter();

        var random = s_Random;
        var seed = random.NextUInt();
        var rockEntities =  m_RockEntities;
        var rockTranslations =  m_RockTranslations;
        var rockScales =  m_RockScales;
        var rockVelocities =  m_RockVelocities;
        Dependency = Entities
            .WithAll<Can>()
            .WithNone<Falling>()
            .WithReadOnly(rockEntities)
            .WithReadOnly(rockTranslations)
            .WithReadOnly(rockScales)
            .WithReadOnly(rockVelocities)
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity velocity, in Translation translation) =>
                CanRocksCollision(ref ecbParaWriter, entity, entityInQueryIndex, ref random, seed, translation,
                    ref velocity, rockEntities, rockTranslations, rockScales, rockVelocities))
            .ScheduleParallel(cullingJob);

        sys.AddJobHandleForProducer(Dependency);
        s_Random = random;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IntersectSphere(float3 p0, float r0, float3 p1, float r1)
    {
        float threshold = r0 + r1;
        return math.distancesq(p0, p1) < threshold * threshold;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IntersectSphereAABB(float3 p0, float r0, AABB aabb)
    {
        return aabb.DistanceSq(p0) < r0 * r0;
    }

    static void AddRockIfInAABB(ref NativeList<Entity>.ParallelWriter entityWriter,
        ref NativeList<Translation>.ParallelWriter translationWriter, ref NativeList<Scale>.ParallelWriter scaleWriter,
        ref NativeList<Velocity>.ParallelWriter velocityWriter, Entity entity, int entityInQueryIndex, AABB aabb,
        Translation translation, Scale scale, Velocity velocity)
    {
        // Rock has a radius of 0.5
        const float radius = 0.5f;
        if (IntersectSphereAABB(translation.Value, radius * scale.Value, aabb))
        {
            entityWriter.AddNoResize(entity);
            translationWriter.AddNoResize(translation);
            scaleWriter.AddNoResize(scale);
            velocityWriter.AddNoResize(velocity);
        }
    }

    static void CanRocksCollision(ref EntityCommandBuffer.ParallelWriter ecb, Entity entity, int entityInQueryIndex,
        ref Random random, uint seed, in Translation canTranslation, ref Velocity canVelocity,
        in NativeList<Entity> rockEntities, in NativeList<Translation> rockTranslations,
        in NativeList<Scale> rockScales, in NativeList<Velocity> rockVelocities)
    {
        if (rockEntities.IsEmpty)
        {
            return;
        }

        // Demo is not physically accurate, collision resolution is done by assigning random velocities

        // Rock has a radius of 0.5
        // Can has a height of 2 and radius of 0.5 -> a sphere of radius 1 can cover the whole can
        const float rockRadius = 0.5f;
        const float canRadius = 1.0f;
        const float canScale = 1.0f; // Ignore the scale of cans spawning in

        // Assert.AreEqual(rockEntities.Length, rockTranslations.Length);
        // Assert.AreEqual(rockEntities.Length, rockScales.Length);
        // Assert.AreEqual(rockEntities.Length, rockVelocities.Length);

        random.InitState(seed * ((uint) entityInQueryIndex + 0x12345678));

        var canHit = false;
        float3 rockVelocity = float3.zero;

        for (int i = 0; i < rockEntities.Length; ++i)
        {
            if (IntersectSphere(canTranslation.Value, canRadius * canScale, rockTranslations[i].Value,
                rockRadius * rockScales[i].Value))
            {
                canHit = true;
                rockVelocity = rockVelocities[i].Value;
                ecb.SetComponent<Velocity>(entityInQueryIndex, rockEntities[i],
                    new Velocity {Value = random.NextFloat3(-1.5f, 1.5f)});
            }
        }

        if (canHit)
        {
            ecb.RemoveComponent<Available>(entityInQueryIndex, entity);
            ecb.AddComponent<Falling>(entityInQueryIndex, entity);
            ecb.AddComponent(entityInQueryIndex, entity, new AngularVelocity
                {Value = math.length(rockVelocity) * random.NextFloat3(-20.0f, 20.0f)});
            canVelocity.Value = rockVelocity;
        }
    }
}
