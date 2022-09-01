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
        ECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

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
        
        NativeArray<Entity> resourceEntities = FreeResourcesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> teamABeeEntities = TeamABeesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> teamBBeeEntities = TeamBBeesQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> teamABeeTranslations = TeamABeesQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<Translation> teamBBeeTranslations = TeamBBeesQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<int> cellResourceStackCount = World.GetOrCreateSystem<ResourceSystem>().CellResourceStackCount;
        
        // Bee spawning
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .ForEach((Entity entity, ref DynamicBuffer<BeeSpawnEvent> spawnEvents) =>
            {
                float3 mapCenter = runtimeData.GridCharacteristics.GetMapCenter();
                    
                for (int i = 0; i < spawnEvents.Length; i++)
                {
                    Entity newBee = ecb.Instantiate(globalData.BeePrefab);
                    BeeSpawnEvent evnt = spawnEvents[i];

                    float3 directionToCenter = math.normalizesafe(mapCenter - evnt.Position);

                    Bee bee = GetComponent<Bee>(globalData.BeePrefab);
                    bee.Random = Unity.Mathematics.Random.CreateFromIndex((uint)i);
                    bee.Velocity =  bee.Random.NextFloat3Direction() * bee.Random.NextFloat(0f, bee.MaxSpawnSpeed);
                    ecb.SetComponent(newBee, bee);
                    
                    ecb.SetComponent(newBee, new Translation { Value = evnt.Position });
                    ecb.SetComponent(newBee, new NonUniformScale { Value = bee.Random.NextFloat(bee.MinBeeSize, bee.MaxBeeSize) });
                    ecb.SetComponent(newBee, new Rotation { Value = quaternion.LookRotationSafe(directionToCenter, math.up())});

                    if (evnt.Team == 1)
                    {
                        ecb.AddComponent(newBee, new BeeState { Team = Team.TeamA });
                        ecb.AddComponent(newBee, new TeamA());
                        ecb.SetComponent(newBee, new OverridableMaterial_Color() { Value = GameUtilities.ColorToFloat4(globalData.TeamAColor) });
                    }
                    else if (evnt.Team == 2)
                    {
                        ecb.AddComponent(newBee, new BeeState { Team = Team.TeamB });
                        ecb.AddComponent(newBee, new TeamB());
                        ecb.SetComponent(newBee, new OverridableMaterial_Color() { Value = GameUtilities.ColorToFloat4(globalData.TeamBColor) });
                    }
                    else
                    {
                        ecb.AddComponent(newBee, new BeeState { Team = Team.None });
                    }
                }
                spawnEvents.Clear();
            }).Schedule();
        
        // Selection of behaviour : If no enemy or resource targeted, choose one based on aggression
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithNone<BeeTargetEnemy>()
            .WithNone<BeeTargetResource>()
            .ForEach((Entity entity, ref Bee bee, in BeeState beeState) => 
            {
                NativeArray<Entity> enemyTeamEntities = default;
                if (beeState.Team == Team.TeamA)
                {
                    enemyTeamEntities = teamBBeeEntities;
                }
                else if (beeState.Team == Team.TeamB)
                {
                    enemyTeamEntities = teamABeeEntities;
                }

                if (enemyTeamEntities.IsCreated)
                {
                    if (bee.Random.NextFloat(0f, 1f) > bee.Aggression)
                    {
                        ecb.AddComponent(entity, new BeeTargetEnemy { Target = enemyTeamEntities[bee.Random.NextInt(0, enemyTeamEntities.Length - 1)] });
                    }
                    else
                    {
                        ecb.AddComponent(entity, new BeeTargetResource { Target = resourceEntities[bee.Random.NextInt(0, resourceEntities.Length - 1)] });
                    }
                }
            }).Schedule();
        
        // Seek resource behaviour
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithNone<BeeTargetEnemy>()
            .WithNone<BeeCarryingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee bee, ref BeeTargetResource targetResource, in Translation translation, in BeeState beeState) =>
            {
                Resource resource = default;
                
                // if resource is picked up, invalid, or not top of stack, cancel
                bool resourceValid = true;
                if (HasComponent<Resource>(targetResource.Target))
                {
                    // if resource has another carrier
                    if (HasComponent<ResourceCarrier>(targetResource.Target) && GetComponent<ResourceCarrier>(targetResource.Target).Carrier != entity)
                    {
                        resourceValid = false;
                    }
                    else
                    {
                        // if resource is not top of stack
                        resource = GetComponent<Resource>(targetResource.Target);
                        if (resource.StackIndex == cellResourceStackCount[resource.CellIndex] - 1)
                        {
                            resourceValid = false;
                        }
                    }
                }
                else
                {
                    resourceValid = false;
                }

                if (resourceValid)
                {
                    // if has non-grabbed resource target, move towards it
                    if (targetResource.IsPickedUp == 0)
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
                    // if has grabbed resource target, move back to base and drop
                    else
                    {
                        // Select our team zone
                        Box selfTeamZone = default;
                        bool validTeamZone = true;
                        if (beeState.Team == Team.TeamA)
                        {
                            selfTeamZone = runtimeData.GridCharacteristics.TeamABounds;
                        }
                        else if (beeState.Team == Team.TeamB)
                        {
                            selfTeamZone = runtimeData.GridCharacteristics.TeamBBounds;
                        }
                        else
                        {
                            validTeamZone = false;
                        }

                        // Drop resource when inside our team zone
                        if (selfTeamZone.IsInside(translation.Value))
                        {
                            
                        }
                        // Move towards team zone
                        else
                        {
                            
                            
                        }
                    }
                }
                else
                {
                    ecb.RemoveComponent<BeeTargetResource>(entity);
                }
            }).ScheduleParallel();
        
        // Bring back resource behaviour
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithNone<BeeTargetEnemy>()
            .WithAll<BeeCarryingResource>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee bee, ref BeeTargetResource targetResource, in Translation translation, in BeeState beeState) =>
            {
            }).ScheduleParallel();
        
        // TargetEnemy behaviour
        ecbParallel = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref Bee bee) => 
            {
                // TODO: if has enemy target, move toward is with chase force
            }).ScheduleParallel();
        
        // Movement with velocity
        Entities
            .WithNativeDisableParallelForRestriction(teamABeeTranslations)
            .WithNativeDisableParallelForRestriction(teamBBeeTranslations)
            .ForEach((ref Translation translation, ref Bee bee, ref Rotation rotation, in BeeState beeState) => 
            {
                // Jitter
                bee.Velocity += bee.Random.NextFloat3Direction() * (bee.FlightJitter * deltaTime);
                
                // Damping
                bee.Velocity *= (1f - bee.Damping);

                // Attract / Repel
                {
                    NativeArray<Translation> selfTeamTranslations = default;
                    if (beeState.Team == Team.TeamA)
                    {
                        selfTeamTranslations = teamABeeTranslations;
                    }
                    else if (beeState.Team == Team.TeamB)
                    {
                        selfTeamTranslations = teamBBeeTranslations;
                    }

                    if (selfTeamTranslations.IsCreated)
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
                
            }).ScheduleParallel();
        
        // Squash & stretch
        Entities
            .ForEach((ref NonUniformScale scale, in Bee bee) => 
            {
                scale.Value.z = math.clamp(math.length(bee.Velocity) * bee.SpeedStretch, 0.5f, 3f);
            }).ScheduleParallel();
        
        // Bee Death
        ecb = ECBSystem.CreateCommandBuffer();
        Entities
            .WithNone<Bee>()
            .ForEach((Entity entity, in BeeState beeState) => 
            {
                // TODO: VFX
                
                ecb.RemoveComponent<BeeState>(entity);
            }).Schedule();

        resourceEntities.Dispose(Dependency);
        teamABeeEntities.Dispose(Dependency);
        teamBBeeEntities.Dispose(Dependency);
        teamABeeTranslations.Dispose(Dependency);
        teamBBeeTranslations.Dispose(Dependency);
        
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
