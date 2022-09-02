using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(ConstrainToLevelBoundsSystem))]
public partial class ResourceSystem : SystemBase
{
    public NativeArray<int> CellResourceStackCount;
    
    private EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (CellResourceStackCount.IsCreated)
        {
            CellResourceStackCount.Dispose();
        }
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<GameGlobalData>() || 
            !HasSingleton<GameRuntimeData>() ||
            !CellResourceStackCount.IsCreated)
            return;

        EntityCommandBuffer ecb;
        EntityCommandBuffer.ParallelWriter ecbParallel;
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        GameGlobalData globalData = GetSingleton<GameGlobalData>();
        GameRuntimeData runtimeData = GetSingleton<GameRuntimeData>();
        Entity runtimeDataEntity = GetSingletonEntity<GameRuntimeData>();
        NativeArray<int> cellResourceStackCount = CellResourceStackCount;
        ComponentDataFromEntity<Translation> translationFromEntity = GetComponentDataFromEntity<Translation>(false);
        BufferFromEntity<BeeSpawnEvent> beeSpawnBufferFromEntity = GetBufferFromEntity<BeeSpawnEvent>(false);

        // Resource spawning
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithName("ResourceSpawnJob")
            .ForEach((Entity entity, ref DynamicBuffer<ResourceSpawnEvent> spawnEvents) =>
            {
                for (int i = 0; i < spawnEvents.Length; i++)
                {
                    Entity newResource = ecb.Instantiate(globalData.ResourcePrefab);
                    ResourceSpawnEvent evnt = spawnEvents[i];
                    ecb.SetComponent(newResource, new Translation { Value = evnt.Position });
                }
                spawnEvents.Clear();
            }).Schedule();
        
        // Make unsettled carrier-less resources fall with gravity
        Entities
            .WithName("ResourceFallingJob")
            .WithNone<ResourceCarrier>()
            .WithNone<ResourceSettled>()
            .ForEach((ref Translation translation, ref Resource resource) =>
            {
                resource.Velocity += math.up() * globalData.Gravity * deltaTime;
                translation.Value += resource.Velocity * deltaTime;
            }).ScheduleParallel();
        
        // Detect reaching ground or stacking or scoring
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithName("ResourceSettlingJob")
            .WithNone<ResourceCarrier>()
            .WithNone<ResourceSettled>()
            .ForEach((Entity entity, ref Translation translation, ref Resource resource) =>
            {
                // Always constrain to bounds
                translation.Value = runtimeData.GridCharacteristics.LevelBounds.GetClosestPoint(translation.Value);
                
                int2 cellCoords = runtimeData.GridCharacteristics.GetCellCoordinatesOfPosition(translation.Value);
                int cellIndex = runtimeData.GridCharacteristics.GetIndexOfCellCoordinates(cellCoords);
                int currentStackCount = cellResourceStackCount[cellIndex];
                float expectedFloorHeight = currentStackCount * globalData.ResourceHeight;
                
                if (translation.Value.y <= expectedFloorHeight)
                {
                    float3 cellPos = runtimeData.GridCharacteristics.GetPositionOfCell(cellCoords);
                    
                    ecb.AddComponent(entity, new ResourceSettled { CellIndex = cellIndex, StackIndex = currentStackCount });
                    ecb.AddComponent(entity, new ResourceSnapToCell { SnapStartTime = time, StartPos = translation.Value, TargetPos = cellPos });
                    
                    translation.Value.y = expectedFloorHeight;
                    resource.Velocity = default;

                    cellResourceStackCount[cellIndex] = currentStackCount + 1;
                    
                    // if landed in a team zone, destroy resources, spawn particles, and spawn bees
                    CellType cellType = runtimeData.GridCharacteristics.GetTypeOfCell(cellCoords);
                    if (cellType == CellType.TeamA || cellType == CellType.TeamB)
                    {
                        ecb.DestroyEntity(entity);
                        
                        // VFX
                        DynamicBuffer<ParticleSpawnEvent> particleEventsBuffer = GetBuffer<ParticleSpawnEvent>(runtimeDataEntity);
                        for (int i = 0; i < globalData.Particles_SparksCount; i++)
                        {
                            particleEventsBuffer.Add(new ParticleSpawnEvent
                            {
                                ParticleType = ParticleType.Spark,
                                Position = translation.Value,
                                Rotation = quaternion.identity,
                                LifetimeRandomization = 0.3f,
                                SizeRandomization = 0.3f,
                                VelocityDirection = math.up(),
                                VelocityMagnitude = globalData.Particles_SparksVelocity,
                                VelocityMagnitudeRandomization = 0.5f,
                                VelocityDirectionRandomizationAngles = 90f,
                            });
                        }
                        for (int i = 0; i < globalData.Particles_SmokesCount; i++)
                        {
                            particleEventsBuffer.Add(new ParticleSpawnEvent
                            {
                                ParticleType = ParticleType.Smoke,
                                Position = translation.Value,
                                Rotation = quaternion.identity,
                                LifetimeRandomization = 0.3f,
                                SizeRandomization = 0.3f,
                                VelocityDirection = math.up(),
                                VelocityMagnitude = globalData.Particles_SmokesVelocity,
                                VelocityMagnitudeRandomization = 0.5f,
                                VelocityDirectionRandomizationAngles = 90f,
                            });
                        }
                        
                        // Spawn bees
                        {
                            Team selectedTeam = default;
                            if (cellType == CellType.TeamA)
                            {
                                selectedTeam = Team.TeamA;
                            }
                            else
                            {
                                selectedTeam = Team.TeamB;
                            }

                            DynamicBuffer<BeeSpawnEvent> beeSpawnEvents = beeSpawnBufferFromEntity[runtimeDataEntity];
                            for (int i = 0; i < globalData.BeeSpawnCountOnScore; i++)
                            {
                                beeSpawnEvents.Add(new BeeSpawnEvent
                                {
                                    Position = translation.Value,
                                    Team = selectedTeam,
                                });
                            }
                        }
                    }
                }
            }).Schedule(); // This one is not parallel because 2 resources may want to write to same index of NativeArray
        
        // Make resources snap to their cell lane over time
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("ResourceSnapToCellJob")
            .WithAll<ResourceSettled>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in Resource resource, in ResourceSnapToCell resourceSnap) =>
            {
                float timeSinceSnapStart = time - resourceSnap.SnapStartTime;
                float timeRatio = timeSinceSnapStart / globalData.ResourceSnapTime;
                float timeRatioClamped = math.clamp(timeRatio, 0f, 1f);

                translation.Value.x = math.lerp(resourceSnap.StartPos.x, resourceSnap.TargetPos.x, timeRatioClamped); 
                translation.Value.z = math.lerp(resourceSnap.StartPos.z, resourceSnap.TargetPos.z, timeRatioClamped);

                if (timeRatio >= 1f)
                {
                    translation.Value.x = resourceSnap.TargetPos.x;
                    translation.Value.z = resourceSnap.TargetPos.z;
                    ecbParallel.RemoveComponent<ResourceSnapToCell>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();
        
        // Make carried resources follow their carrier
        Entities
            .WithName("ResourceFollowCarrierJob")
            .WithNativeDisableParallelForRestriction(translationFromEntity) // safe because we only ever write to self entity
            .ForEach((Entity entity, ref Resource resource, in ResourceCarrier carrier) => 
            {
                if (translationFromEntity.HasComponent(carrier.Carrier))
                {
                    Translation selfTranslation = translationFromEntity[entity];
                    
                    float3 initialPos = selfTranslation.Value;
                    float3 carrierPos = translationFromEntity[carrier.Carrier].Value;
                    float3 targetPos = carrierPos - (math.up() * globalData.ResourceCarryOffset);
                    selfTranslation.Value = math.lerp(selfTranslation.Value, targetPos, globalData.ResourceCarryStiffness * deltaTime);
                    resource.Velocity = (selfTranslation.Value - initialPos) / deltaTime;

                    translationFromEntity[entity] = selfTranslation;
                }
            }).ScheduleParallel();
        
        // Make settled resources remove themselves from stacks when carried
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithName("ResourceRemoveFromStackJob")
            .WithAll<ResourceCarrier>()
            .ForEach((Entity entity, ref Resource resource, in ResourceSettled resourceSettled) => 
            {
                int currentStackCount = cellResourceStackCount[resourceSettled.CellIndex];
                cellResourceStackCount[resourceSettled.CellIndex] = math.max(0, currentStackCount - 1);
                ecb.RemoveComponent<ResourceSettled>(entity);
            }).Schedule();
        
        // Drop resource if carrier died
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("ResourceDropWhenNullCarrierJob")
            .ForEach((Entity entity, int entityInQueryIndex, ref Resource resource, in ResourceCarrier carrier) => 
            {
                if (!HasComponent<Bee>(carrier.Carrier))
                {
                    GameUtilities.DropResource(entity, carrier.Carrier, ecbParallel, entityInQueryIndex);
                }
            }).ScheduleParallel();
        
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
