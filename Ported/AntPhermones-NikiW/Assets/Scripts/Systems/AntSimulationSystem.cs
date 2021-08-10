using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using Random = UnityEngine.Random;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))] 
public class AntSimulationSystem : SystemBase
{
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
    
    AntQueriesSystem m_AntQueriesSystem;

    protected override void OnCreate()
    { 
        Debug.Log($"'{World.Name}' {this}.OnCreate!");
        
        RequireSingletonForUpdate<AntSimulationParams>();
        RequireSingletonForUpdate<AntSimulationRuntimeData>();
        RequireSingletonForUpdate<AntSimulationPrefabs>();
        m_EndFixedStepSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        m_AntQueriesSystem = World.GetOrCreateSystem<AntQueriesSystem>();
       
        counters = new NativeArray<long>(3, Allocator.Persistent);
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

        Debug.Log($"'{World.Name}' {this}.OnStartRunning!");
        
        m_ObstacleManagementSystem = World.GetExistingSystem<ObstacleManagementSystem>();
        
        // Init DynamicBuffers:
        using var pheromonesBufferDatas = new NativeArray<FoodPheromonesBufferData>(simParams.mapSize * simParams.mapSize, Allocator.Temp);
        var foodPheromonesBufferDatas = this.GetSingletonBuffer<FoodPheromonesBufferData>();
        foodPheromonesBufferDatas.AddRange(pheromonesBufferDatas);
        var colonyPheromonesBufferDatas = this.GetSingletonBuffer<ColonyPheromonesBufferData>();
        colonyPheromonesBufferDatas.AddRange(pheromonesBufferDatas.Reinterpret<ColonyPheromonesBufferData>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if(counters.IsCreated) counters.Dispose();
    }

    protected override unsafe void OnUpdate()
    {
        var simStart = UnityEngine.Time.realtimeSinceStartupAsDouble;

        var simParams = GetSingleton<AntSimulationParams>();
        var simRuntimeData = GetSingleton<AntSimulationRuntimeData>();
        var prefabs = GetSingleton<AntSimulationPrefabs>();

        // NW: Need local variables to satisfy Entities Lambda.
        var countersLocal = counters;
        var obstacleCollisionLookupLocal = m_ObstacleManagementSystem.obstacleCollisionLookup;
        if (!obstacleCollisionLookupLocal.IsCreated)
            return;
        
        var ecb = m_EndFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        if (!simRuntimeData.hasSpawnedAnts)
        {
            simRuntimeData.hasSpawnedAnts = true;
            SetSingleton(simRuntimeData);

            Debug.Log($"'{World.Name}' spawning {simParams.antCount} ants...");
            
            var antEntities = EntityManager.Instantiate(prefabs.npcAntPrefab, simParams.antCount, Allocator.TempJob);
            var minPos = new float2();
            var maxPos = new float2(simParams.mapSize, simParams.mapSize);
            Debug.Assert(HasComponent<AntSimulationTransform2D>(prefabs.npcAntPrefab), "antPrefab MUST have a Transform2D!");
            for (var i = 0; i < antEntities.Length; i++)
            {
                var antEntity = antEntities[i];
                EntityManager.SetComponentData(antEntity, new LifeTicks
                {
                    value = (ushort)(Random.value * 30)
                });
                EntityManager.SetComponentData(antEntity, new AntSimulationTransform2D
                {
                    position = math.clamp(simRuntimeData.colonyPos + (float2)Random.insideUnitCircle * simParams.colonyRadius,minPos, maxPos),
                    facingAngle = (ushort)(Random.value * math.PI * 2)
                });
            }

            antEntities.Dispose(Dependency);
        }

        var perFrameRandomSeed = AntSimulationUtilities.GeneratePerFrameRandomSeed();
        Dependency = Entities
            .WithBurst()
            .ForEach((int nativeThreadIndex, ref AntSimulationTransform2D translation, ref LifeTicks lifeTicks) =>
            {
                if (lifeTicks.value <= 0)
                {
                    // Ant died!
                    var seed = (uint)translation.position.x ^ (uint)translation.position.y;
                    translation.position = simRuntimeData.colonyPos + new float2(math.cos(translation.facingAngle), math.sin(translation.facingAngle)) * Squirrel3.NextFloat(seed, perFrameRandomSeed, 0f, simParams.colonyRadius);
                    lifeTicks.value = (ushort)(Squirrel3.NextDouble(seed, perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);
                    Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[2]);
                }

                lifeTicks.value--;
            }).ScheduleParallel(Dependency);

        Dependency.Complete();
        
        var pheromonesFoodLocal = this.GetSingletonBuffer<FoodPheromonesBufferData>().AsNativeArray().Reinterpret<float>();
        var pheromonesColonyLocal = this.GetSingletonBuffer<ColonyPheromonesBufferData>().AsNativeArray().Reinterpret<float>();
        
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
            IsHoldingFoodTypeHandle = GetComponentTypeHandle<IsHoldingFoodFlag>(true),
            perFrameRandomSeed = perFrameRandomSeed,
        }.ScheduleParallel(m_AntQueriesSystem.antsQuery, 1, Dependency);

        m_EndFixedStepSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        RunDropPheromonesJob(simParams, m_AntQueriesSystem.antsSearchingQuery, pheromonesColonyLocal, simParams.pheromoneAddSpeedWhenSearching);
        RunDropPheromonesJob(simParams, m_AntQueriesSystem.antsHoldingFoodQuery, pheromonesFoodLocal, simParams.pheromoneAddSpeedWithFood);

        Dependency.Complete();
       
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
        if (m_AntQueriesSystem.antsQuery == default) return "Waiting for Query...";

        var antsCount = m_AntQueriesSystem.antsQuery.CalculateEntityCount();
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

        [ReadOnly]
        public uint perFrameRandomSeed;

        public unsafe void Execute(ArchetypeChunk chunk, int batchIndex)
        {
            Debug.Assert(chunk.Has(LifeTicksTypeHandle), "No LifeTicks!");
            Debug.Assert(chunk.Has(Transform2DTypeHandle), "No Transform2D!");
            
            var entitiesPtr = chunk.GetNativeArray(EntityTypeHandle);
            var lifeTicksPtr = chunk.GetNativeArray(LifeTicksTypeHandle).GetUnsafePtr();
            var transformsPtr = chunk.GetNativeArray(Transform2DTypeHandle).GetUnsafePtr();
            
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
                ref var trans = ref UnsafeUtility.ArrayElementAsRef<AntSimulationTransform2D>(transformsPtr, i);
                var entity = entitiesPtr[i];
                
                // NW: Add some random rotation to indicate "curiosity"...
                var randRotation = Squirrel3.NextFloat((uint)entity.Index, perFrameRandomSeed, -simParams.randomSteeringStrength, simParams.randomSteeringStrength);
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
                        ref var lifeTicks = ref UnsafeUtility.ArrayElementAsRef<LifeTicks>(lifeTicksPtr, i);
                        
                        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                        if (isHoldingFood)
                        {
                            ecb.RemoveComponent<IsHoldingFoodFlag>(entity.Index, entity);
                            Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[1]);

                            
                            lifeTicks.value = (ushort)(Squirrel3.NextFloat((uint)entity.Index, perFrameRandomSeed, 0.5f, 1.5f) * simParams.ticksForAntToDie);
                            trans.facingAngle += Squirrel3.NextFloat((uint)entity.Index, perFrameRandomSeed, -math.PI, math.PI);
                        }
                        else
                        {
                            ecb.AddComponent<IsHoldingFoodFlag>(entity.Index, entity);
                            Interlocked.Increment(ref ((long*)countersLocal.GetUnsafePtr())[0]);
                            
                            lifeTicks.value += (ushort)(Squirrel3.NextFloat((uint)entity.Index, perFrameRandomSeed, 0.25f, 0.75f) * simParams.ticksForAntToDie);
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

            if (Unity.Burst.CompilerServices.Hint.Likely(math.any(isInBounds)))
            {
                ref var value = ref UnsafeUtility.ArrayElementAsRef<float>(pheromones.GetUnsafePtr(), pheromoneIndex);
                var excitement = pheromoneAddSpeed;
                value += excitement * simulationDt;
            }
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
