using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///     NW: Converted <see cref="AntSimulationRenderSystem" /> to this:
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AntSimulationSystem : SystemBase
{
    const float k_PIX2 = math.PI * 2f;

    public EntityQuery antsHoldingFoodQuery;
    public EntityQuery antsQuery;
    public EntityQuery antsSearchingQuery;
    /// <summary>
    ///     0 = Number of times food was picked up.
    ///     1 = Number of times food was dropped off.
    ///     2 = Number of times ants have died.
    /// </summary>
    [NoAlias]
    public NativeArray<long> counters;
    EndFixedStepSimulationEntityCommandBufferSystem m_EndFixedStepSimulationEntityCommandBufferSystem;

    ObstacleManagementSystem m_ObstacleManagementSystem;

    AverageState m_SimulationElapsedSeconds;

    [NoAlias]
    public NativeArray<float> pheromonesColony;
    [NoAlias]
    public NativeArray<float> pheromonesFood;

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

        antsHoldingFoodQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.ReadOnly<IsHoldingFoodFlag>());
        antsSearchingQuery = GetEntityQuery(ComponentType.ReadOnly<AntSimulationTransform2D>(), ComponentType.ReadOnly<AntBehaviourFlag>(), ComponentType.Exclude<IsHoldingFoodFlag>());
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
                    facingAngle = (ushort)(Random.value * math.PI * 2)
                });
            }

            antEntities.Dispose(Dependency);
        }

        Dependency = Entities
            .WithBurst()
            .ForEach((int nativeThreadIndex, ref AntSimulationTransform2D translation, ref LifeTicks lifeTicks) =>
            {
                if (lifeTicks.value <= 0)
                {
                    // Ant died!
                    var seed = (uint)translation.position.x ^ (uint)translation.position.y;
                    translation.position = simRuntimeData.colonyPos + new float2(math.cos(translation.facingAngle), math.sin(translation.facingAngle)) * Squirrel3.NextFloat(seed, simRuntimeData.perFrameRandomSeed, 0f, simParams.colonyRadius);
                    lifeTicks.value = (ushort)(Squirrel3.NextDouble(seed, simRuntimeData.perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);
                    Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[2]);
                }

                lifeTicks.value--;
            }).ScheduleParallel(Dependency);

        Dependency = new AntSteeringSimulationJob
        {
            ecb = ecb,
            countersLocal = countersLocal,
            simParams = simParams,
            simRuntimeData = simRuntimeData,
            pheromonesColonyLocal = pheromonesColonyLocal,
            pheromonesFoodLocal = pheromonesFoodLocal,
            obstacleCollisionLookupLocal = obstacleCollisionLookupLocal,
            EntityTypeHandle = GetEntityTypeHandle(),
            LifeTicksTypeHandle = GetComponentTypeHandle<LifeTicks>(),
            Transform2DTypeHandle = GetComponentTypeHandle<AntSimulationTransform2D>(),
            IsHoldingFoodTypeHandle = GetComponentTypeHandle<IsHoldingFoodFlag>(true)
        }.ScheduleParallel(antsQuery, 1, Dependency);

        m_EndFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        RunDropPheromonesJob(simParams, antsSearchingQuery, pheromonesColonyLocal, simParams.pheromoneAddSpeedWhenSearching);
        RunDropPheromonesJob(simParams, antsHoldingFoodQuery, pheromonesFoodLocal, simParams.pheromoneAddSpeedWithFood);

        var weaken1 = new WeakenPotencyOfPheromonesJob
        {
            pheromones = pheromonesColonyLocal,
            pheromoneDecay = simParams.pheromoneDecay
        }.Schedule(pheromonesColonyLocal.Length, 8, Dependency);
        var weaken2 = new WeakenPotencyOfPheromonesJob
        {
            pheromones = pheromonesFoodLocal,
            pheromoneDecay = simParams.pheromoneDecay
        }.Schedule(pheromonesFoodLocal.Length, 8, Dependency);

        Dependency = JobHandle.CombineDependencies(weaken1, weaken2);

        m_SimulationElapsedSeconds.AddSample(UnityEngine.Time.realtimeSinceStartupAsDouble - simStart);
    }

    void RunDropPheromonesJob(in AntSimulationParams simParams, in EntityQuery query, NativeArray<float> pheromonesArray, in float pheromoneAddSpeed)
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

    public string DumpStatusText()
    {
        if (antsQuery == default) return "Waiting for Query...";

        var antsCount = antsQuery.CalculateEntityCount();
        var found = counters[0];
        var gathered = counters[1];
        var gatheredPerc = (float)gathered / found;
        return $"Ants: {antsCount} ({counters[2]} respawned)\nFood Found: {found}, Gathered: {gathered} ({gatheredPerc:p2})\nSim CPU: {m_SimulationElapsedSeconds.Average * 1000_000_000:#,000}ns per Sim";
    }

    [BurstCompile]
    [NoAlias]
    public struct AntSteeringSimulationJob : IJobEntityBatch
    {
        [NoAlias]
        public ComponentTypeHandle<AntSimulationTransform2D> Transform2DTypeHandle;

        [NoAlias]
        public ComponentTypeHandle<LifeTicks> LifeTicksTypeHandle;

        [ReadOnly]
        [NoAlias]
        public EntityTypeHandle EntityTypeHandle;

        [ReadOnly]
        [NoAlias]
        public ComponentTypeHandle<IsHoldingFoodFlag> IsHoldingFoodTypeHandle;

        [ReadOnly]
        [NoAlias]
        public AntSimulationParams simParams;

        [ReadOnly]
        [NoAlias]
        public AntSimulationRuntimeData simRuntimeData;

        [NoAlias]
        [ReadOnly]
        public NativeBitArray obstacleCollisionLookupLocal;

        [NoAlias]
        [ReadOnly]
        public NativeArray<float> pheromonesFoodLocal, pheromonesColonyLocal;

        [NoAlias]
        public EntityCommandBuffer.ParallelWriter ecb;

        [NoAlias]
        public NativeArray<long> countersLocal;

        public unsafe void Execute(ArchetypeChunk chunk, int batchIndex)
        {
            var entitiesPtr = chunk.GetNativeArray(EntityTypeHandle);
            var lifeTicksPtr = (LifeTicks*)chunk.GetNativeArray(LifeTicksTypeHandle).GetUnsafePtr();
            var transformsPtr = (AntSimulationTransform2D*)chunk.GetNativeArray(Transform2DTypeHandle).GetUnsafePtr();

            var isHoldingFood = chunk.Has(IsHoldingFoodTypeHandle);
            
            NativeArray<float> pheromones;
            float steerStrength;
            float antSpeed;
            float2 targetPos;
            if (isHoldingFood)
            {
                pheromones = pheromonesColonyLocal;
                steerStrength = simParams.pheromoneSteerStrengthWithFood;
                targetPos = simRuntimeData.colonyPos;
                antSpeed = simParams.antSpeedHoldingFood;
            }
            else
            {
                pheromones = pheromonesFoodLocal;
                steerStrength = simParams.pheromoneSteerStrengthWhenSearching;
                targetPos = simRuntimeData.foodPosition;
                antSpeed = simParams.antSpeedSearching;
            }

            for (var i = 0; i < chunk.Count; i++)
            {
                ref var trans = ref transformsPtr[i];
                var entity = entitiesPtr[i];
                
                // NW: Add some random rotation to indicate "curiosity"...
                var randRotation = Squirrel3.NextFloat((uint)entity.Index, simRuntimeData.perFrameRandomSeed, -simParams.randomSteeringStrength, simParams.randomSteeringStrength);
                trans.facingAngle += randRotation;

                // Avoid walls:
                {
                    float wallSteering = 0;
                    const float wallSteeringTestDistance = 1.5f;
                    for (var i2 = -1; i2 <= 1; i2 += 2)
                    {
                        var angle1 = trans.facingAngle + i2 * math.PI * .25f;
                        var testX1 = (int)(trans.position.x + math.cos(angle1) * wallSteeringTestDistance);
                        var testY1 = (int)(trans.position.y + math.sin(angle1) * wallSteeringTestDistance);

                        var isInBounds2 = AntSimulationUtilities.CalculateIsInBounds(in testX1, in testY1, in simParams.mapSize, out var obstacleCollisionIndex);

                        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                        if (!math.any(isInBounds2) || math.all(isInBounds2) && obstacleCollisionLookupLocal.IsSet(obstacleCollisionIndex))
                            wallSteering -= i2;

                        //Debug.DrawLine((Vector2)ant.position / mapSize, new Vector2(testX, testY) / mapSize, Color.red, 0.2f);
                    }

                    trans.facingAngle += wallSteering * simParams.wallSteerStrength;
                }

                // Steer towards target if the ant can "see" it.
                var steerTowardsTargetWeight = simParams.colonySteerStrength;
                if (DirectPathToTargetIsBlocked(in trans.position, in targetPos, in simParams.mapSize, obstacleCollisionLookupLocal, out var distanceToTarget))
                {
                    // Pheromone steering:
                    {
                        const float pheromoneSteerCheckAhead = 3;
                        const float quarterPi = math.PI * .25f;
                        var output = 0f;
                        for (var i1 = -1; i1 <= 1; i1 += 2)
                        {
                            var angle = trans.facingAngle + i1 * quarterPi;
                            var testX = (int)(trans.position.x + math.cos(angle) * pheromoneSteerCheckAhead);
                            var testY = (int)(trans.position.y + math.sin(angle) * pheromoneSteerCheckAhead);
                            var isInBoundsPheromones = AntSimulationUtilities.CalculateIsInBounds(in testX, in testY, in simParams.mapSize, out var index);
                            if (math.all(isInBoundsPheromones)) output += pheromones[index] * i1;
                        }

                        trans.facingAngle += math.sign(output) * steerStrength;
                    }
                }
                else
                {
                    // Steer towards the target.
                    steerTowardsTargetWeight = simParams.seenTargetSteerStrength;

                    // Pick up / Drop off food only when we're within LOS and within distance.
                    if (distanceToTarget < simParams.colonyRadius)
                    {
                        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                        if (isHoldingFood)
                        {
                            ecb.RemoveComponent<IsHoldingFoodFlag>(entity.Index, entity);
                            Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[1]);
                            
                            lifeTicksPtr[i].value = (ushort)(Squirrel3.NextFloat((uint)entity.Index, simRuntimeData.perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);
                            trans.facingAngle += Squirrel3.NextFloat((uint)entity.Index, simRuntimeData.perFrameRandomSeed, -math.PI, math.PI);
                        }
                        else
                        {
                            ecb.AddComponent<IsHoldingFoodFlag>(entity.Index, entity);
                            Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[0]);
                            
                            lifeTicksPtr[i].value += (ushort)(Squirrel3.NextFloat((uint)entity.Index, simRuntimeData.perFrameRandomSeed, 0.25f, 0.75f) * simParams.ticksForAntToDie);
                            trans.facingAngle += math.PI;
                        }
                    }
                }

                // Head towards a target.
                // Much more weight if we've seen it.
                // Much more weight if it's a known location (colony). E.g. Simulating memory.
                var targetAngle = math.atan2(targetPos.y - trans.position.y, targetPos.x - trans.position.x);

                // if (targetAngle - trans.facingAngle > math.PI)
                // {
                //     trans.facingAngle -= math.PI;
                // }
                // else if (targetAngle - trans.facingAngle < -math.PI)
                // {
                //     trans.facingAngle += math.PI;
                // }
                trans.facingAngle += math.clamp(targetAngle - trans.facingAngle, -steerTowardsTargetWeight, steerTowardsTargetWeight);

                var velocity = new float2(math.cos(trans.facingAngle), math.sin(trans.facingAngle));
                velocity = math.normalizesafe(velocity) * antSpeed * 0.6f;

                // Move, then undo it later if we moved into invalid.
                var newPos = trans.position + velocity;

                // NW: If we're now colliding with a wall, bounce:
                var isInBounds = AntSimulationUtilities.CalculateIsInBounds(in newPos, in simParams.mapSize, out var collisionIndex);
                if (!math.all(isInBounds))
                    trans.facingAngle = math.atan2(math.@select(-velocity.y, velocity.y, isInBounds.y), math.@select(-velocity.x, velocity.x, isInBounds.x));
                else if (obstacleCollisionLookupLocal.IsSet(collisionIndex))
                    trans.facingAngle += math.PI;

                //Debug.DrawLine((Vector2)trans.position / mapSize, (Vector2)(trans.position + velocity) / mapSize, Color.red, 0.2f);
                else
                    trans.position = newPos;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool DirectPathToTargetIsBlocked(in float2 origin, in float2 target, in int mapSize, NativeBitArray obstacleCollisionLookup, out float dist)
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
    public unsafe struct WeakenPotencyOfPheromonesJob : IJobParallelFor
    {
        [NoAlias]
        public NativeArray<float> pheromones;

        public float pheromoneDecay;

        public void Execute(int index)
        {
            var unsafePtr = pheromones.GetUnsafePtr();
            ref var pheromone = ref UnsafeUtility.ArrayElementAsRef<float>(unsafePtr, index);
            pheromone = math.clamp(pheromone * pheromoneDecay, 0, 1);
        }
    }
}
