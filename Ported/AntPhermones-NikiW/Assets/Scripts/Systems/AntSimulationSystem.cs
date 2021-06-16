using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///     NW: Converted <see cref="AntSimulationRenderSystem" /> to this:
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AntSimulationSystem : SystemBase
{
    const float k_PIX2 = math.PI * 2f;
    static ProfilerMarker s_SetHoldingFoodMarker = new ProfilerMarker("SetHoldingFoodFlag");
    public EntityQuery antsQuery;
    /// <summary>
    ///     0 = Number of times food was picked up.
    ///     1 = Number of times food was dropped off.
    ///     2 = Number of times ants have died.
    /// </summary>
    [NoAlias]
    public NativeArray<long> counters;

    EntityQuery m_AntsHoldingFoodQuery;
    EntityQuery m_AntsSearchingQuery;

    ObstacleManagementSystem m_ObstacleManagementSystem;

    [NoAlias]
    public NativeArray<float> pheromonesColony;
    [NoAlias]
    public NativeArray<float> pheromonesFood;
    
    AverageState simulationElapsedSeconds;
    EndFixedStepSimulationEntityCommandBufferSystem m_EndFixedStepSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<AntSimulationParams>();
        RequireSingletonForUpdate<AntSimulationRuntimeData>();
        m_EndFixedStepSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        if (!TryGetSingleton(out AntSimulationParams simParams))
        {
            Debug.LogError("Unable to find AntSimulationParams, so creating a new one!");
            simParams = AntSimulationParams.Default;
            EntityManager.CreateEntity(ComponentType.ReadWrite<AntSimulationParams>());
            SetSingleton(simParams);
        }

        Debug.Log($"Initializing {this}!");
        counters = new NativeArray<long>(3, Allocator.Persistent);
        pheromonesColony = new NativeArray<float>(simParams.mapSize * simParams.mapSize, Allocator.Persistent);
        pheromonesFood = new NativeArray<float>(simParams.mapSize * simParams.mapSize, Allocator.Persistent);
        
        m_ObstacleManagementSystem = World.GetOrCreateSystem<ObstacleManagementSystem>();

        m_AntsHoldingFoodQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.ReadOnly<IsHoldingFoodFlag>());
        m_AntsSearchingQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.Exclude<IsHoldingFoodFlag>());
        antsQuery = GetEntityQuery(ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.ReadOnly<AntSimulationTransform2D>());
    }

    protected override unsafe void OnUpdate()
    {
        var simStart = UnityEngine.Time.realtimeSinceStartupAsDouble;

        var simParams = GetSingleton<AntSimulationParams>();
        var simRuntimeData = GetSingleton<AntSimulationRuntimeData>();

        // NW: Need local variables to satisfy Entities Lambda.
        var countersLocal = counters;
        var obstacleCollisionLookupLocal = m_ObstacleManagementSystem.obstacleCollisionLookup;
        var pheromonesFoodLocal = pheromonesFood;
        var pheromonesColonyLocal = pheromonesColony;
        var ecb = m_EndFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        if (!simRuntimeData.hasSpawnedAnts)
        {
            simRuntimeData.hasSpawnedAnts = true;
            SetSingleton(simRuntimeData);

            var antEntities = EntityManager.Instantiate(simParams.antPrefab, simParams.antCount, Allocator.TempJob);
            Debug.Assert(HasComponent<AntSimulationTransform2D>(simParams.antPrefab), "antPrefab MUST have a Transform2D!");
            for (var i = 0; i < antEntities.Length; i++)
            {
                var antEntity = antEntities[i];
                EntityManager.AddComponentData(antEntity, new LifeTicks
                {
                    value = (ushort)(Random.value * 30)
                });
                EntityManager.SetComponentData(antEntity, new AntSimulationTransform2D
                {
                    position = simRuntimeData.colonyPos + (float2)Random.insideUnitCircle * simParams.colonyRadius,
                    facingAngle = (ushort)(Random.value * math.PI * 2),
                });
            }

            antEntities.Dispose(Dependency);
        }

        Dependency = Entities
            .WithBurst()
            .ForEach((Entity entity, ref AntSimulationTransform2D translation, ref LifeTicks lifeTicks) =>
            {
                if (lifeTicks.value <= 0)
                {
                    // Ant died!
                    translation.position = simRuntimeData.colonyPos;
                    lifeTicks.value = (ushort)(Squirrel3.NextFloat((uint)entity.Index, simRuntimeData.perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);

                    ref var count = ref ((long*)countersLocal.GetUnsafePtr())[2];
                    count++;
                }

                lifeTicks.value--;
            }).ScheduleParallel(Dependency);

        Dependency = Entities
            .WithBurst()
            .WithReadOnly(obstacleCollisionLookupLocal)
            .WithReadOnly(pheromonesColonyLocal)
            .WithAll<AntBehaviourFlag, IsHoldingFoodFlag>()
            .ForEach((Entity entity, int nativeThreadIndex, ref AntSimulationTransform2D trans, ref LifeTicks lifeTicks) =>
            {
                UpdateAntBaseSteering(ref trans, in entity, in simParams, in simRuntimeData, in obstacleCollisionLookupLocal);
                UpdateAntLinecastSteering(in entity, in nativeThreadIndex, ref trans, ref lifeTicks, in simParams, in simRuntimeData, obstacleCollisionLookupLocal, pheromonesColonyLocal, in simParams.pheromoneSteerStrengthWithFood, ecb, in simParams.antSpeedHoldingFood, in simRuntimeData.colonyPos, true);
            }).ScheduleParallel(Dependency);

        Dependency = Entities
            .WithBurst()
            .WithReadOnly(obstacleCollisionLookupLocal)
            .WithReadOnly(pheromonesFoodLocal)
            .WithAll<AntBehaviourFlag>()
            .WithNone<IsHoldingFoodFlag>()
            .ForEach((Entity entity, int nativeThreadIndex, ref AntSimulationTransform2D trans, ref LifeTicks lifeTicks) =>
            {
                UpdateAntBaseSteering(ref trans, in entity, in simParams, in simRuntimeData, in obstacleCollisionLookupLocal);
                UpdateAntLinecastSteering(in entity, in nativeThreadIndex,ref trans, ref lifeTicks, in simParams, in simRuntimeData, obstacleCollisionLookupLocal, pheromonesFoodLocal, in simParams.pheromoneSteerStrengthWhenSearching, ecb, in simParams.antSpeedSearching, in simRuntimeData.foodPosition, false);
            }).ScheduleParallel(Dependency);

        m_EndFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        
        RunDropPheromonesJob(simParams, m_AntsSearchingQuery, pheromonesColonyLocal, simParams.pheromoneAddSpeedWhenSearching);
        RunDropPheromonesJob(simParams, m_AntsHoldingFoodQuery, pheromonesFoodLocal, simParams.pheromoneAddSpeedWithFood);
        
        var weaken1 = new WeakenPotencyOfPheromonesJob
        {
            pheromones = pheromonesColonyLocal,
            pheromoneDecay = simParams.pheromoneDecay
        }.Schedule(Dependency);
        var weaken2 = new WeakenPotencyOfPheromonesJob
        {
            pheromones = pheromonesFoodLocal,
            pheromoneDecay = simParams.pheromoneDecay
        }.Schedule(Dependency);

        Dependency = JobHandle.CombineDependencies(weaken1, weaken2);
        
        simulationElapsedSeconds.AddSample(UnityEngine.Time.realtimeSinceStartupAsDouble - simStart);
    }

    void RunDropPheromonesJob(in AntSimulationParams simParams, in EntityQuery query, in NativeArray<float> pheromonesArray, in float pheromoneAddSpeed)
    {
        if (query.IsEmpty) return;

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        var transform2Ds = query.ToComponentDataArray<AntSimulationTransform2D>(Allocator.TempJob);
        Dependency = new AntDropPheromonesJob
        {
            dropPositions = transform2Ds,
            pheromones = pheromonesArray,
            simulationDt = Time.DeltaTime,

            pheromoneAddSpeed = pheromoneAddSpeed,
            mapSize = simParams.mapSize,
            ticksForAntToDie = simParams.ticksForAntToDie
        }.Schedule(transform2Ds.Length, 8, Dependency);

        Dependency.Complete();

        transform2Ds.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void UpdateAntLinecastSteering(in Entity entity, in int nativeThreadIndex, ref AntSimulationTransform2D trans, ref LifeTicks lifeTicks, [ReadOnly] in AntSimulationParams simParams, in AntSimulationRuntimeData runtimeData, [ReadOnly] NativeBitArray obstacleCollisionLookup, [ReadOnly] NativeArray<float> pheromones, in float pheromoneSteerStrength, EntityCommandBuffer.ParallelWriter ecb, in float antSpeed, in float2 targetPos, bool isHoldingFood)
    {
        // Steer towards target if the ant can "see" it.
        var steerTowardsTargetWeight = simParams.colonySteerStrength;
        if (DirectPathToTargetIsBlocked(in trans.position, in targetPos, in simParams.mapSize, in obstacleCollisionLookup, out var distanceToTarget))
        {
            // Steer out of the way of obstacles and map boundaries if we can't see the target.
            var pheroSteering = PheromoneSteering(ref trans, in pheromones, in simParams.mapSize);
            trans.facingAngle += pheroSteering * pheromoneSteerStrength;
        }
        else
        {
            // Steer towards the target.
            steerTowardsTargetWeight = simParams.seenTargetSteerStrength;

            // Pick up / Drop off food only when we're within LOS and within distance.
            if (distanceToTarget < simParams.colonyRadius)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                if(isHoldingFood)
                    ecb.RemoveComponent<IsHoldingFoodFlag>(nativeThreadIndex, entity);
                else ecb.AddComponent<IsHoldingFoodFlag>(nativeThreadIndex, entity);

                // The ant either found food or returned home with it, so it gets to live.
                lifeTicks.value = (ushort)(Squirrel3.NextFloat((uint)entity.Index, runtimeData.perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);
            }
        }

        // Head towards a target.
        // Much more weight if we've seen it.
        // Much more weight if it's a known location (colony). E.g. Simulating memory.
        var targetAngle = math.atan2(targetPos.y - trans.position.y, targetPos.x - trans.position.x);
        if (targetAngle - trans.facingAngle > math.PI)
        {
            trans.facingAngle -= k_PIX2;
        }
        else if (targetAngle - trans.facingAngle < -math.PI)
        {
            trans.facingAngle += k_PIX2;
        }
        else
        {
            if (math.abs(targetAngle - trans.facingAngle) < math.PI * .5f) trans.facingAngle += (targetAngle - trans.facingAngle) * steerTowardsTargetWeight;
        }

        var velocity = new float2(math.cos(trans.facingAngle), math.sin(trans.facingAngle));
        velocity = math.normalizesafe(velocity) * antSpeed * 0.6f;

        // Move, then undo it later if we moved into invalid.
        var newPos = trans.position + velocity;

        // NW: If we're now colliding with a wall, bounce:
        var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in newPos, in simParams.mapSize, out var collisionIndex);
        if (!math.all(isInBounds))
            trans.facingAngle = math.atan2(math.select(-velocity.y, velocity.y, isInBounds.y), math.select(-velocity.x, velocity.x, isInBounds.x));
        else if (obstacleCollisionLookup.IsSet(collisionIndex))
            trans.facingAngle += math.PI;

        //Debug.DrawLine((Vector2)trans.position / mapSize, (Vector2)(trans.position + velocity) / mapSize, Color.red, 0.2f);
        else
            trans.position = newPos;
    }

    static void UpdateAntBaseSteering(ref AntSimulationTransform2D trans, in Entity entity, in AntSimulationParams simParams, in AntSimulationRuntimeData runtimeData, in NativeBitArray obstacleCollisionLookup)
    {
        // NW: Add some random rotation to indicate "curiosity"...
        var randRotation = Squirrel3.NextFloat((uint)entity.Index, runtimeData.perFrameRandomSeed, -simParams.randomSteeringStrength, simParams.randomSteeringStrength);
        trans.facingAngle += randRotation;

        // Avoid walls:
        var wallSteering = WallSteering(in trans, in simParams.mapSize, in obstacleCollisionLookup);
        trans.facingAngle += wallSteering * simParams.wallSteerStrength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float PheromoneSteering(ref AntSimulationTransform2D trans, [ReadOnly] in NativeArray<float> pheromones, in int mapSize)
    {
        const float pheromoneSteerCheckAhead = 3;
        const float quarterPi = math.PI * .25f;
        var output = 0f;
        for (var i = -1; i <= 1; i += 2)
        {
            var angle = trans.facingAngle + i * quarterPi;
            var testX = (int)(trans.position.x + math.cos(angle) * pheromoneSteerCheckAhead);
            var testY = (int)(trans.position.y + math.sin(angle) * pheromoneSteerCheckAhead);
            var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in testX, in testY, in mapSize, out var index);
            if (math.all(isInBounds)) output += pheromones[index] * i;
        }

        return math.sign(output);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int WallSteering(in AntSimulationTransform2D trans, in int mapSize, in NativeBitArray obstacleCollisionLookup)
    {
        const float wallSteeringTestDistance = 1.5f;
        var output = 0;
        for (var i = -1; i <= 1; i += 2)
        {
            var angle = trans.facingAngle + i * math.PI * .25f;
            var testX = (int)(trans.position.x + math.cos(angle) * wallSteeringTestDistance);
            var testY = (int)(trans.position.y + math.sin(angle) * wallSteeringTestDistance);

            var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in testX, in testY, in mapSize, out var obstacleCollisionIndex);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (!math.any(isInBounds) || math.all(isInBounds) && obstacleCollisionLookup.IsSet(obstacleCollisionIndex))
                output -= i;

            //Debug.DrawLine((Vector2)ant.position / mapSize, new Vector2(testX, testY) / mapSize, Color.red, 0.2f);
        }

        return output;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool DirectPathToTargetIsBlocked(in float2 origin, in float2 target, in int mapSize, in NativeBitArray obstacleCollisionLookup, out float dist)
    {
        var dx = target.x - origin.x;
        var dy = target.y - origin.y;
        dist = math.sqrt(dx * dx + dy * dy);

        // NW: Test to see if step count can be generalized.
        var stepCount = Mathf.CeilToInt(dist * .5f);
        for (var i = 0; i < stepCount; i++)
        {
            var t = (float)i / stepCount;
            var testX = (int)(origin.x + dx * t);
            var testY = (int)(origin.y + dy * t);
            var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in testX, in testY, in mapSize, out var collisionIndex);

            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            if (!math.all(isInBounds) || obstacleCollisionLookup.IsSet(collisionIndex))
                return true;
        }

        return false;
    }

    public string DumpStatusText()
    {
        if (antsQuery == default) return "Waiting for Query...";

        var antsCount = antsQuery.CalculateEntityCount();
        return $" Ants: {antsCount} ({counters[2]} respawned)\nFood Found: {counters[0]}, Gathered: {counters[1]}\nSim CPU: {simulationElapsedSeconds.Average * 1000_000_000:#,000}ns per Sim";
    }

    [BurstCompile]
    [NoAlias]
    public struct AntDropPheromonesJob : IJobParallelFor
    {
        [NoAlias]
        [ReadOnly]
        public NativeArray<AntSimulationTransform2D> dropPositions;

        [NoAlias]
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<float> pheromones;

        public float simulationDt, pheromoneAddSpeed;
        public int mapSize;
        public ushort ticksForAntToDie;

        public unsafe void Execute(int index)
        {
            // NW: Ants spread pheromones in tiles around their current pos.
            // Writing to same array locations, so be aware that we're clobbering data here!
            // However, after discussion on slack, it's an acceptable tradeoff.
            var position = dropPositions[index].position;
            var x = Mathf.RoundToInt(position.x);
            var y = Mathf.RoundToInt(position.y);

            var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in x, in y, in mapSize, out var pheromoneIndex);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!math.any(isInBounds))
                throw new InvalidOperationException("Attempting to DropPheromones outside the map bounds!");
#endif

            ref var value = ref UnsafeUtility.ArrayElementAsRef<float>(pheromones.GetUnsafePtr(), pheromoneIndex);
            var excitement = pheromoneAddSpeed;
            value += excitement * simulationDt;
        }
    }

    [BurstCompile]
    [NoAlias]
    public unsafe struct WeakenPotencyOfPheromonesJob : IJob
    {
        [NoAlias]
        public NativeArray<float> pheromones;

        public float pheromoneDecay;

        public void Execute()
        {
            var unsafePtr = pheromones.GetUnsafePtr();
            for (var index = 0; index < pheromones.Length; index++)
            {
                ref var pheromone = ref UnsafeUtility.ArrayElementAsRef<float>(unsafePtr, index);
                pheromone = math.clamp(pheromone * pheromoneDecay, 0, 1);
            }
        }
    }
}
