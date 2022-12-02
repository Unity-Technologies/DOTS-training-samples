using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct BeeBehaviourSystem : ISystem
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
        var seed = (uint)(state.WorldUnmanaged.Time.ElapsedTime * 1000);
        var random = Random.CreateFromIndex(seed);
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
        var up = new float3(0f, 1f, 0f);

        var hive0Bees = new DynamicBuffer<TargetBee>();
        var hive1Bees = new DynamicBuffer<TargetBee>();

        foreach (var (hiveBees, team) in SystemAPI.Query<DynamicBuffer<TargetBee>, Team>())
        {
            if(team.number == 0)
            {
                hive0Bees = hiveBees;
            }
            else
            {
                hive1Bees = hiveBees;
            }
        }

        var availableResources = SystemAPI.GetSingletonBuffer<AvailableResources>();

        if (hive0Bees.Length == 0 && hive1Bees.Length == 0)
            return;

        var behaviourJob = new BeeBehaviourJob
        {
            transformLookup = SystemAPI.GetComponentLookup<WorldTransform>(),
            resourceLookup = SystemAPI.GetComponentLookup<Resource>(),
            beeStateLookup = SystemAPI.GetComponentLookup<BeeState>(),
            beeTargetLookup = SystemAPI.GetComponentLookup<BeeTarget>(),
            resourceCarriedLookup = SystemAPI.GetComponentLookup<ResourceCarried>(),
            resourceDroppedLookup = SystemAPI.GetComponentLookup<ResourceDropped>(),
            entityStorageInfo = SystemAPI.GetEntityStorageInfoLookup(),
            config = config,
            ecb = ecb.AsParallelWriter(),
            up = up,
            seed = seed,
            deltaTime = deltaTime,
            availableResources = availableResources,
            hive0Bees = hive0Bees,
            hive1Bees = hive1Bees
        };
        behaviourJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BeeBehaviourJob : IJobEntity
{
    [NativeDisableContainerSafetyRestriction][ReadOnly] public ComponentLookup<WorldTransform> transformLookup;
    [ReadOnly] public ComponentLookup<Resource> resourceLookup;
    [NativeDisableContainerSafetyRestriction][ReadOnly] public ComponentLookup<BeeState> beeStateLookup;
    [NativeDisableContainerSafetyRestriction][ReadOnly] public ComponentLookup<BeeTarget> beeTargetLookup;
    [ReadOnly] public ComponentLookup<ResourceCarried> resourceCarriedLookup;
    [ReadOnly] public ComponentLookup<ResourceDropped> resourceDroppedLookup;
    [ReadOnly] public EntityStorageInfoLookup entityStorageInfo;

    public EntityCommandBuffer.ParallelWriter ecb;
    public uint seed;
    public float deltaTime;
    public Config config;
    [NativeDisableContainerSafetyRestriction][Unity.Collections.ReadOnly] public DynamicBuffer<TargetBee> hive0Bees;
    [NativeDisableContainerSafetyRestriction][Unity.Collections.ReadOnly] public DynamicBuffer<TargetBee> hive1Bees;
    [Unity.Collections.ReadOnly] public DynamicBuffer<AvailableResources> availableResources;
    public float3 up;

    private void Execute([ChunkIndexInQuery] int chunkIndex, TransformAspect transform, RefRW<BeeState> bee, Team team, RefRW<BeeTarget> target, Entity entity)
    {
        var random = Random.CreateFromIndex(seed + (uint)entity.Index);
        var velocity = bee.ValueRO.velocity;
        var randomDirection = random.NextFloat3Direction();
        velocity += randomDirection * (config.flightJitter * deltaTime);
        velocity *= (1f - config.damping);

        var allies = team.number == 0 ? hive0Bees : hive1Bees;
        var enemies = team.number == 0 ? hive1Bees : hive0Bees;
        if (allies.Length > 0 && bee.ValueRO.beeState != BeeStateEnumerator.Dying)
        {
            var attractiveFriend = allies[random.NextInt(0, allies.Length)];
            var delta = attractiveFriend.position - transform.WorldPosition;
            var dist = math.sqrt((delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z));
            if (dist > 0f)
            {
                velocity += delta * (config.teamAttraction * deltaTime / dist);
            }

            var repellentFriend = allies[random.NextInt(0, allies.Length)];
            delta = repellentFriend.position - transform.WorldPosition;
            dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            if (dist > 0f)
            {
                velocity -= delta * (config.teamRepulsion * deltaTime / dist);
            }
        }

        switch (bee.ValueRO.beeState)
        {
            case BeeStateEnumerator.Idle:
                if (random.NextFloat(1f) < config.aggression) 
                {
                    if (enemies.Length > 0)
                    {
                        var enemy = enemies[random.NextInt(0, enemies.Length)];
                        target.ValueRW.target = enemy.enemy;
                        bee.ValueRW.beeState = BeeStateEnumerator.Attacking;
                    }
                } 
                else 
                {
                    if (availableResources.Length > 0)
                    {
                        var resource = availableResources[random.NextInt(0, availableResources.Length)];
                        target.ValueRW.target = resource.resource;
                        bee.ValueRW.beeState = BeeStateEnumerator.Gathering;
                    }
                }
                break;
            case BeeStateEnumerator.Attacking:
            {
                
                if (!target.IsValid || target.ValueRO.target == Entity.Null ||
                    !entityStorageInfo.Exists(target.ValueRO.target) || !transformLookup.HasComponent(target.ValueRO.target))
                {
                    bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                }
                else
                {
                    var enemyTransform = transformLookup.GetRefRO(target.ValueRO.target);
                    var enemyState = beeStateLookup.GetRefRO(target.ValueRO.target);
                    var delta = enemyTransform.ValueRO.Position - transform.WorldPosition;
                    float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                    if (sqrDist > config.attackDistance * config.attackDistance)
                    {
                        velocity += delta * (config.chaseForce * deltaTime / math.sqrt(sqrDist));
                    }
                    else
                    {
                        velocity += delta * (config.attackForce * deltaTime / math.sqrt(sqrDist));
                        if (sqrDist < config.hitDistance * config.hitDistance)
                        {
                            if (enemyState.ValueRO.beeState == BeeStateEnumerator.CarryBack)
                            {
                                var enemyTarget = beeTargetLookup.GetRefRO(target.ValueRO.target);
                                if (enemyTarget.ValueRO.target != Entity.Null && resourceLookup.HasComponent(enemyTarget.ValueRO.target) && entityStorageInfo.Exists(enemyTarget.ValueRO.target))
                                {
                                    var resource = resourceLookup.GetRefRO(enemyTarget.ValueRO.target);
                                    var resourceVal = resource.ValueRO;
                                    resourceVal.ownerBee = Entity.Null;
                                    ecb.SetComponent(chunkIndex, enemyTarget.ValueRO.target, resourceVal);
                                    ecb.SetComponentEnabled<ResourceCarried>(chunkIndex, enemyTarget.ValueRO.target, false);
                                }
                            }

                            var enemyStateVal = enemyState.ValueRO;
                            enemyStateVal.beeState = BeeStateEnumerator.Dying;
                            enemyStateVal.deathTimer = 1f;
                            enemyStateVal.velocity *= .5f;
                            ecb.SetComponent(chunkIndex, target.ValueRO.target, enemyStateVal);
                            target.ValueRW.target = Entity.Null;
                            bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                        }
                    }
                }

                break;
            }
            case BeeStateEnumerator.Gathering:
            {
                if (!target.IsValid || target.ValueRO.target == Entity.Null ||
                    !entityStorageInfo.Exists(target.ValueRO.target) ||
                    !resourceLookup.HasComponent(target.ValueRO.target))
                {
                    bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                }
                else
                {
                    var isCarried = resourceCarriedLookup.IsComponentEnabled(target.ValueRO.target);
                    if (isCarried)
                    {
                        bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                    }
                    else
                    {
                        var targetTransform = transformLookup.GetRefRO(target.ValueRO.target);
                        var delta = targetTransform.ValueRO.Position - transform.WorldPosition;
                        var sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > config.grabDistance * config.grabDistance)
                        {
                            velocity += delta * (config.chaseForce * deltaTime / math.sqrt(sqrDist));
                        }
                        else
                        {
                            //Debug.Log($"Grabbing resource {target.ValueRO.targetPosition} - {transform.WorldPosition}, dist {sqrDist}");
                            var resource = resourceLookup.GetRefRO(target.ValueRO.target);
                            var resourceVal = resource.ValueRO;
                            resourceVal.ownerBee = entity;
                            ecb.SetComponent(chunkIndex, target.ValueRO.target, resourceVal);
                            ecb.SetComponentEnabled<ResourceCarried>(chunkIndex, target.ValueRO.target, true);
                            bee.ValueRW.beeState = BeeStateEnumerator.CarryBack;
                        }
                    }
                }

                break;
            }
            case BeeStateEnumerator.CarryBack:
            {
                var targetPos = new float3(-config.fieldSize.x * .45f + config.fieldSize.x * .9f * team.number, 0f,
                    transform.WorldPosition.z);
                var delta = targetPos - transform.WorldPosition;
                var dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                velocity += (targetPos - transform.WorldPosition) * (config.carryForce * deltaTime / dist);
                if (dist < 1f)
                {
                    if (target.IsValid && entityStorageInfo.Exists(target.ValueRO.target) && resourceLookup.HasComponent(target.ValueRO.target))
                    {
                        var resource = resourceLookup.GetRefRO(target.ValueRO.target);
                        var resourceVal = resource.ValueRO;
                        resourceVal.ownerBee = Entity.Null;
                        ecb.SetComponent(chunkIndex, target.ValueRO.target, resourceVal);
                        ecb.SetComponentEnabled<ResourceCarried>(chunkIndex, target.ValueRO.target, false);
                        ecb.SetComponentEnabled<ResourceDropped>(chunkIndex, target.ValueRO.target, true);
                    }

                    bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                }

                break;
            }
            case BeeStateEnumerator.Dying:
                if(bee.ValueRW.deathTimer >= 1f)
                    SpawnParticles(chunkIndex, config, ecb, transform.LocalPosition, 5);
                bee.ValueRW.deathTimer -= deltaTime;

                if(bee.ValueRW.deathTimer < 0f)
                    ecb.DestroyEntity(chunkIndex, entity);
                break;
        }
        
        // Apply velocity to position
        transform.WorldPosition += velocity * deltaTime;

        var worldPosition = transform.WorldPosition;
        if (math.abs(worldPosition.x) > config.fieldSize.x * .5f) {
            worldPosition.x = (config.fieldSize.x * .5f) * math.sign(transform.WorldPosition.x);
            velocity.x *= -.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }
        if (math.abs(worldPosition.z) > config.fieldSize.z * .5f) {
            worldPosition.z = (config.fieldSize.z * .5f) * math.sign(worldPosition.z);
            velocity.z *= -.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        float resourceModifier = 0f;
        if (bee.ValueRO.beeState == BeeStateEnumerator.CarryBack) {
            resourceModifier = .4f;
        }
        if (math.abs(worldPosition.y) > config.fieldSize.y * .5f - resourceModifier) {
            worldPosition.y = (config.fieldSize.y * .5f - resourceModifier) * math.sign(worldPosition.y);
            velocity.y *= -.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
        }

        // Apply position, rotation & velocity
        transform.WorldPosition = worldPosition;
        transform.WorldPosition += velocity * deltaTime;
        transform.WorldRotation = quaternion.LookRotation(math.normalize(bee.ValueRW.velocity), up);
        bee.ValueRW.velocity = velocity;
    }

    private static void SpawnParticles(int chunkIndex, Config config, EntityCommandBuffer.ParallelWriter ecb, float3 position, int count)
    {
        var particles = new NativeArray<Entity>(count, Allocator.Temp);
        ecb.Instantiate(chunkIndex, config.particlePrefab, particles);
        var color = new URPMaterialPropertyBaseColor { Value = new float4(0.8f, 0f, 0f, 1f) };
        var random = new Random();
        random.InitState((uint)position.GetHashCode());
        foreach (var particle in particles)
        {
            ecb.SetComponent(chunkIndex, particle, color);
            var scale = random.NextFloat(.75f, 1f);
            ecb.SetComponent(chunkIndex, particle, new LocalTransform
            {
                Position = position,
                Scale = scale,
                Rotation = quaternion.identity
            });
            ecb.SetComponent(chunkIndex, particle, new WorldTransform()
            {
                Position = position,
                Scale = scale,
                Rotation = quaternion.identity
            });
            ecb.SetComponent(chunkIndex, particle, new Particle()
            {
                life = 1f,
                lifeTime = random.NextFloat(.75f, 2f),
                velocity = random.NextFloat3Direction() * 5f,
                size = scale
            });
            ecb.AddComponent(chunkIndex, particle, new PostTransformScale()
            {
                Value = float3x3.Scale(scale)
            });
        }
    }
}