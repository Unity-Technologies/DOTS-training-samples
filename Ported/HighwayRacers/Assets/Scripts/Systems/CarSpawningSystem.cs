using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct CarSpawningSystem : ISystem
{
    private EntityQuery m_CarQuery;
    private EntityQuery m_BaseColorQuery;

    public bool NeedsRegenerating
    {
        get => _needsRegenerating;
        set => _needsRegenerating = value;
    }
    private bool _needsRegenerating;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CarConfig>();
        state.RequireForUpdate<TrackConfig>();
        m_CarQuery = state.GetEntityQuery(typeof(CarPosition));
        m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));

        NeedsRegenerating = true;
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (NeedsRegenerating) {
            CarGlobalColors globalColors = SystemAPI.GetSingleton<CarGlobalColors>();
            var config = SystemAPI.GetSingleton<CarConfig>();
            var trackConfig = SystemAPI.GetSingleton<TrackConfig>();

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
            var vehicles = CollectionHelper.CreateNativeArray<Entity>(trackConfig.numberOfCars, allocator);
            ecb.Instantiate(config.CarPrefab, vehicles);

            var queryMask = m_BaseColorQuery.GetEntityQueryMask();

            var random = Unity.Mathematics.Random.CreateFromIndex(501);

            foreach (var vehicle in vehicles)
            {
                int lane = random.NextInt(4);
                ecb.SetComponent(vehicle, new CarPosition
                {
                    distance = random.NextFloat(0, trackConfig.highwaySize),
                    currentLane = lane
                });

                ecb.SetComponent(vehicle, new CarChangingLanes
                {
                    FromLane = lane,
                    ToLane = lane,
                    Progress = 0
                });

                // setting current and desired to be the same on spawn. Current is what is modified from AI and tries to match desired
                float localDesiredSpeed = random.NextFloat(config.MinDefaultSpeed, config.MaxDefaultSpeed);

                ecb.SetComponent(vehicle, new CarSpeed
                {
                    currentSpeed = localDesiredSpeed
                });

                ecb.SetComponent(vehicle, new CarProperties
                {
                    desiredSpeed = localDesiredSpeed,
                    overTakePercent = random.NextFloat(config.MinOvertakeSpeedScale, config.MaxOvertakeSpeedScale),
                    minDistanceInFront = random.NextFloat(config.MinDistanceInFront, config.MaxDistanceInFront),
                    mergeSpace = random.NextFloat(config.MinMergeSpace, config.MaxMergeSpace),
                    overTakeEagerness = random.NextFloat(config.MinOvertakeEagerness, config.MaxOvertakeEagerness),
                    defaultSpeed = random.NextFloat(config.MinDefaultSpeed, config.MaxDefaultSpeed),
                    leftMergeDistance = random.NextFloat(config.MinLeftMergeDistance, config.MaxLeftMergeDistance),
                    acceleration = 15,
                    braking = 20,
                });

                ecb.SetComponent(vehicle, new CarColor
                {
                    currentColor = globalColors.defaultColor
                });

                ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, new URPMaterialPropertyBaseColor { Value = (Vector4)globalColors.defaultColor });
            }

            NeedsRegenerating = false;
        }
    }

    public void RespawnCars(EntityManager entityManager)
    {
        // Remove the tracks that have already been created (if any)
        NativeArray<Entity> entitiesToRemove = m_CarQuery.ToEntityArray(Allocator.Temp);
        foreach (Entity entity in entitiesToRemove)
        {
            entityManager.DestroyEntity(entity);
        }
        entitiesToRemove.Dispose();

        NeedsRegenerating = true;
    }
}
