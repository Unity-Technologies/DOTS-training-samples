using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(ConstrainToLevelBoundsSystem))]
public partial class BeeSystem : SystemBase
{
    private EntityCommandBufferSystem ECBSystem;
    private EntityQuery TeamABeesQuery;
    private EntityQuery TeamBBeesQuery;
    private EntityQuery FreeResourcesQuery;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        ECBSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        TeamABeesQuery = GetEntityQuery(typeof(Bee), typeof(TeamA), typeof(Translation));
        TeamBBeesQuery = GetEntityQuery(typeof(Bee), typeof(TeamB), typeof(Translation));
        FreeResourcesQuery = GetEntityQuery(typeof(Resource), typeof(Translation), typeof(ResourceSettled), ComponentType.Exclude(typeof(ResourceCarrier)));
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<GameGlobalData>() || 
            !HasSingleton<GameRuntimeData>())
            return;
        
        EntityCommandBuffer ecb;
        EntityCommandBuffer.ParallelWriter ecbParallel;
        float deltaTime = Time.DeltaTime;
        float time = (float)Time.ElapsedTime;
        GameGlobalData globalData = GetSingleton<GameGlobalData>();
        GameRuntimeData runtimeData = GetSingleton<GameRuntimeData>();
        Entity runtimeDataEntity = GetSingletonEntity<GameRuntimeData>();
        ComponentDataFromEntity<Bee> beeFromEntity = GetComponentDataFromEntity<Bee>(false);

        // TODO: maybe this strategy of allocating tmp arrays isn't the best. But it needs less maintenance than persistent lists
        NativeArray<Entity> resourceEntities = FreeResourcesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> teamABeeEntities = TeamABeesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> teamBBeeEntities = TeamBBeesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> teamABeeTranslations = TeamABeesQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<Translation> teamBBeeTranslations = TeamBBeesQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<int> cellResourceStackCount = World.GetOrCreateSystem<ResourceSystem>().CellResourceStackCount;
        
        // Bee spawning
        ecb = ECBSystem.CreateCommandBuffer();
        Dependency = Entities
            .WithName("BeeSpawnJob")
            .ForEach((Entity entity, ref DynamicBuffer<BeeSpawnEvent> spawnEvents) =>
            {
                float3 mapCenter = runtimeData.GridCharacteristics.GetMapCenter();
                uint spawnCounter = 0;
                
                for (int i = 0; i < spawnEvents.Length; i++)
                {
                    Entity newBee = ecb.Instantiate(globalData.BeePrefab);
                    BeeSpawnEvent evnt = spawnEvents[i];

                    float3 directionToCenter = math.normalizesafe(mapCenter - evnt.Position);

                    Bee bee = GetComponent<Bee>(globalData.BeePrefab);
                    bee.Random = Unity.Mathematics.Random.CreateFromIndex(spawnCounter);
                    bee.Velocity =  bee.Random.NextFloat3Direction() * bee.Random.NextFloat(0f, bee.MaxSpawnSpeed);
                    
                    ecb.SetComponent(newBee, new Translation { Value = evnt.Position });
                    ecb.SetComponent(newBee, new NonUniformScale { Value = bee.Random.NextFloat(bee.MinBeeSize, bee.MaxBeeSize) });
                    ecb.SetComponent(newBee, new Rotation { Value = quaternion.LookRotationSafe(directionToCenter, math.up())});

                    if (evnt.Team == Team.TeamA)
                    {
                        bee.Team = Team.TeamA;
                        ecb.AddComponent(newBee, new TeamA());
                        ecb.SetComponent(newBee, new OverridableMaterial_Color() { Value = GameUtilities.ColorToFloat4(globalData.TeamAColor) });
                    }
                    else if (evnt.Team == Team.TeamB)
                    {
                        bee.Team = Team.TeamB;
                        ecb.AddComponent(newBee, new TeamB());
                        ecb.SetComponent(newBee, new OverridableMaterial_Color() { Value = GameUtilities.ColorToFloat4(globalData.TeamBColor) });
                    }
                    
                    ecb.SetComponent(newBee, bee);
                    spawnCounter++;
                }
                spawnEvents.Clear();
            }).Schedule(Dependency);
        
        // Selection of behaviour : If no enemy or resource targeted, choose one based on aggression
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
            .WithName("BeeBehaviourSelectJob")
            .WithReadOnly(teamABeeEntities)
            .WithReadOnly(teamBBeeEntities)
            .WithReadOnly(resourceEntities)
            .WithNone<BeeTargetEnemy>()
            .WithNone<BeeTargetResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee bee) => 
            {
                NativeArray<Entity> enemyTeamEntities = default;
                if (bee.Team == Team.TeamA)
                {
                    enemyTeamEntities = teamBBeeEntities;
                }
                else if (bee.Team == Team.TeamB)
                {
                    enemyTeamEntities = teamABeeEntities;
                }

                if (enemyTeamEntities.IsCreated && enemyTeamEntities.Length > 0 && bee.Random.NextFloat(0f, 1f) < bee.Aggression)
                {
                    ecbParallel.AddComponent(entityInQueryIndex, entity, new BeeTargetEnemy { Target = enemyTeamEntities[bee.Random.NextInt(0, enemyTeamEntities.Length - 1)] });
                }
                else if (resourceEntities.IsCreated && resourceEntities.Length > 0)
                {
                    ecbParallel.AddComponent(entityInQueryIndex, entity, new BeeTargetResource { Target = resourceEntities[bee.Random.NextInt(0, resourceEntities.Length - 1)] });
                }
            }).ScheduleParallel(Dependency);
        
        // Resource behaviour
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
            .WithName("BeeResourceBehaviourJob")
            .WithReadOnly(cellResourceStackCount)
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee bee, in BeeTargetResource targetResource, in Translation translation) =>
            {
                bool resourceCarrierIsSelf = false;
                bool resourceValid = true;

                // Validate resource
                {
                    if (HasComponent<Resource>(targetResource.Target))
                    {
                        // invalid if resource has another carrier
                        if (HasComponent<ResourceCarrier>(targetResource.Target))
                        {
                            resourceCarrierIsSelf = GetComponent<ResourceCarrier>(targetResource.Target).Carrier == entity;
                            if (!resourceCarrierIsSelf)
                            {
                                resourceValid = false;
                            }
                        }

                        // invalid if resource is settled but is not top of stack
                        if (HasComponent<ResourceSettled>(targetResource.Target))
                        {
                            ResourceSettled resourceSettled = GetComponent<ResourceSettled>(targetResource.Target);
                            if (resourceSettled.StackIndex < cellResourceStackCount[resourceSettled.CellIndex] - 1)
                            {
                                resourceValid = false;
                            }
                        }
                    }
                    // invalid if resource is destroyed
                    else
                    {
                        resourceValid = false;
                    }
                }

                if (resourceValid)
                {
                    // Bring resource back to base if we're the active carrier of that resource
                    if (resourceCarrierIsSelf)
                    {
                        // Select our team zone
                        Box selfTeamZone = default;
                        bool validTeamZone = true;
                        if (bee.Team == Team.TeamA)
                        {
                            selfTeamZone = runtimeData.GridCharacteristics.TeamABounds;
                        }
                        else if (bee.Team == Team.TeamB)
                        {
                            selfTeamZone = runtimeData.GridCharacteristics.TeamBBounds;
                        }
                        else
                        {
                            validTeamZone = false;
                        }

                        if (validTeamZone)
                        {
                            // Drop resource when inside our team zone
                            if (selfTeamZone.IsInside(translation.Value))
                            {
                                GameUtilities.DropResource(targetResource.Target, entity, ecbParallel, entityInQueryIndex);
                            }
                            // Move towards team zone
                            else
                            {
                                Box shrunkenTeamZone = selfTeamZone;
                                shrunkenTeamZone.Extents.xz *= 0.8f;
                                shrunkenTeamZone.Extents.y *= 0.4f;
                                shrunkenTeamZone.Recalculate();

                                float3 destination = shrunkenTeamZone.GetClosestPoint(translation.Value);
                                bee.Velocity += (destination - translation.Value) * (bee.CarryForce * deltaTime / math.distance(translation.Value, destination));
                            }
                        }
                    }
                    // Seek out resource
                    else
                    {
                        float3 delta = GetComponent<Translation>(targetResource.Target).Value - translation.Value;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                        // Move towards resource
                        if (sqrDist > (bee.GrabDistance * bee.GrabDistance))
                        {
                            bee.Velocity += delta * (bee.ChaseForce * deltaTime / math.sqrt(sqrDist));
                        }
                        // pickup resource
                        else
                        {
                            ecbParallel.AddComponent(entityInQueryIndex, targetResource.Target, new ResourceCarrier { Carrier = entity });
                        }
                    }
                }
                else
                {
                    ecbParallel.RemoveComponent<BeeTargetResource>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        // Attack behaviour
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
            .WithName("BeeAttackBehaviourJob")
            .WithNativeDisableParallelForRestriction(beeFromEntity)
            .WithAll<Bee>()
            .ForEach((Entity entity, int entityInQueryIndex, in BeeTargetEnemy targetEnemy) => 
            {
                // if has enemy target, move toward is with chase force
                if (beeFromEntity.HasComponent(targetEnemy.Target))
                { 
                    Bee bee = beeFromEntity[entity];
                    float3 selfTranslation = GetComponent<Translation>(entity).Value;
                    
                    float3 delta = GetComponent<Translation>(targetEnemy.Target).Value - selfTranslation;
                    float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                    if (sqrDist > bee.AttackDistance * bee.AttackDistance)
                    {
                        bee.Velocity += delta * (bee.ChaseForce * deltaTime / math.sqrt(sqrDist));
                    }
                    else
                    {
                        bee.Velocity += delta * (bee.AttackForce * deltaTime / math.sqrt(sqrDist));
                        if (sqrDist < bee.HitDistance * bee.HitDistance)
                        {
                            // TODO: particles
                            
                            ecbParallel.AddComponent(entityInQueryIndex, targetEnemy.Target, new BeeDeath());
                            ecbParallel.RemoveComponent<BeeTargetEnemy>(entityInQueryIndex, entity);
                        }
                    }

                    beeFromEntity[entity] = bee;
                }
                else
                {
                    ecbParallel.RemoveComponent<BeeTargetEnemy>(entityInQueryIndex, entity);
                }
                
            }).ScheduleParallel(Dependency);
        
        // Movement with velocity
        Dependency = Entities
            .WithName("BeeMovementJob")
            .WithNativeDisableParallelForRestriction(teamABeeTranslations)
            .WithNativeDisableParallelForRestriction(teamBBeeTranslations)
            .ForEach((ref Translation translation, ref Bee bee, ref Rotation rotation) => 
            {
                // Jitter
                bee.Velocity += bee.Random.NextFloat3Direction() * (bee.FlightJitter * deltaTime);
                
                // Damping
                bee.Velocity *= (1f - bee.Damping);

                // Attract / Repel
                {
                    NativeArray<Translation> selfTeamTranslations = default;
                    if (bee.Team == Team.TeamA)
                    {
                        selfTeamTranslations = teamABeeTranslations;
                    }
                    else if (bee.Team == Team.TeamB)
                    {
                        selfTeamTranslations = teamBBeeTranslations;
                    }

                    if (selfTeamTranslations.IsCreated && selfTeamTranslations.Length > 0)
                    {
                        // Attractive Friend
                        {
                            float3 delta = selfTeamTranslations[bee.Random.NextInt(0, selfTeamTranslations.Length - 1)].Value - translation.Value;
                            float dist = math.length(delta);
                            if (dist > 0f)
                            {
                                bee.Velocity += delta * (bee.TeamAttraction * deltaTime / dist);
                            }
                        }

                        // Repellant Friend
                        {
                            float3 delta = selfTeamTranslations[bee.Random.NextInt(0, selfTeamTranslations.Length - 1)].Value - translation.Value;
                            float dist = math.length(delta);
                            if (dist > 0f)
                            {
                                bee.Velocity -= delta * (bee.TeamRepulsion * deltaTime / dist);
                            }
                        }
                    }
                }
                
                // Move
                translation.Value += bee.Velocity * deltaTime;
                
                // Rotate towards vel
                rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(math.normalizesafe(bee.Velocity), math.up()), bee.RotationSharpness * deltaTime);
                
            }).ScheduleParallel(Dependency);
        
        // Squash & stretch
        Dependency = Entities
            .WithName("BeeStretchJob")
            .ForEach((ref NonUniformScale scale, in Bee bee) => 
            {
                scale.Value.z = math.clamp(math.length(bee.Velocity) * bee.SpeedStretch, 0.6f, 1.1f);
            }).ScheduleParallel(Dependency);
        
        // Bee Death
        ecb = ECBSystem.CreateCommandBuffer();
        Dependency = Entities
            .WithName("BeeDeathJob")
            .WithAll<BeeDeath>()
            .ForEach((Entity entity, in Bee bee, in Translation translation) => 
            {
                //  VFX
                DynamicBuffer<ParticleSpawnEvent> particleEventsBuffer = GetBuffer<ParticleSpawnEvent>(runtimeDataEntity);
                for (int i = 0; i < globalData.Particles_BloodsCount; i++)
                {
                    particleEventsBuffer.Add(new ParticleSpawnEvent
                    {
                        ParticleType = ParticleType.BloodInFlight,
                        Position = translation.Value,
                        Rotation = quaternion.identity,
                        SizeRandomization = 0.3f,
                        VelocityDirection = math.up(),
                        VelocityMagnitude = globalData.Particles_BloodsVelocity,
                        VelocityMagnitudeRandomization = 0.5f,
                        VelocityDirectionRandomizationAngles = 25f,
                    });
                }
                
                ecb.DestroyEntity(entity);
            }).Schedule(Dependency);

        resourceEntities.Dispose(Dependency);
        teamABeeEntities.Dispose(Dependency);
        teamBBeeEntities.Dispose(Dependency);
        teamABeeTranslations.Dispose(Dependency);
        teamBBeeTranslations.Dispose(Dependency);
        
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
