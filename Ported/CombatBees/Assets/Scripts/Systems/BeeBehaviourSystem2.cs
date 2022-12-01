using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct BeeBehaviourSystem2 : ISystem
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

        var hive0Bees = new DynamicBuffer<EnemyBees>();
        var hive1Bees = new DynamicBuffer<EnemyBees>();

        foreach (var (hiveBees, team) in SystemAPI.Query<DynamicBuffer<EnemyBees>, Team>())
        {
            if(team.number == 1)
            {
                hive0Bees = hiveBees;
            }
            else
            {
                hive1Bees = hiveBees;
            }
        }

        var availableResources = new DynamicBuffer<AvailableResources>();
        foreach (var resources in SystemAPI.Query<DynamicBuffer<AvailableResources>>())
        {
            availableResources = resources;
        }

        if (hive0Bees.Length == 0 && hive1Bees.Length == 0)
            return;
        
        var entitiesToDestroy = new NativeList<Entity>(Allocator.TempJob);

        foreach (var (transform, bee, team, target, entity) in SystemAPI.Query<TransformAspect, RefRW<BeeState>, Team, RefRW<BeeTarget>>().WithEntityAccess())
        {
            var velocity = bee.ValueRO.velocity;
            var randomDirection = random.NextFloat3Direction();
            velocity += randomDirection * (config.flightJitter * deltaTime);
            velocity *= (1f-config.damping);

            var allies = team.number == 0 ? hive0Bees : hive1Bees;
            var enemies = team.number == 0 ? hive1Bees : hive0Bees;
            if (allies.Length > 0)
            {
                var attractiveFriend = allies[random.NextInt(0, allies.Length)];
                var delta = attractiveFriend.enemyPosition - transform.WorldPosition;
                var dist = math.sqrt((delta.x * delta.x) + (delta.y * delta.y) + (delta.z * delta.z));
                if (dist > 0f)
                {
                    velocity += delta * (config.teamAttraction * deltaTime / dist);
                }

                var repellentFriend = allies[random.NextInt(0, allies.Length)];
                delta = repellentFriend.enemyPosition - transform.WorldPosition;
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
                            target.ValueRW.targetPosition = enemy.enemyPosition;
                            bee.ValueRW.beeState = BeeStateEnumerator.Attacking;
                        }
                    } 
                    else 
                    {
                        if (availableResources.Length > 0)
                        {
                            var resource = availableResources[random.NextInt(0, availableResources.Length)];
                            target.ValueRW.target = resource.resource;
                            target.ValueRW.targetPosition = resource.resourcePosition;
                            bee.ValueRW.beeState = BeeStateEnumerator.Gathering;
                        }
                    }
                    break;
                case BeeStateEnumerator.Attacking:
                {
                    if (!target.IsValid || target.ValueRO.target == Entity.Null ||
                        !SystemAPI.Exists(target.ValueRO.target))
                    {
                        bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                    }
                    else
                    {
                        var enemyTransform = SystemAPI.GetAspectRW<TransformAspect>(target.ValueRO.target);
                        var enemyState = SystemAPI.GetComponent<BeeState>(target.ValueRO.target);
                        var delta = enemyTransform.WorldPosition - transform.WorldPosition;
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
                                if (enemyState.beeState == BeeStateEnumerator.CarryBack)
                                {
                                    var enemyTarget = SystemAPI.GetComponent<BeeTarget>(target.ValueRO.target);
                                    if (enemyTarget.target != Entity.Null && SystemAPI.HasComponent<Resource>(enemyTarget.target))
                                    {
                                        var resource = SystemAPI.GetComponent<Resource>(enemyTarget.target);
                                        resource.ownerBee = Entity.Null;
                                        SystemAPI.SetComponent(enemyTarget.target, resource);
                                        SystemAPI.SetComponentEnabled<ResourceCarried>(enemyTarget.target, false);
                                    }
                                }
                                enemyState.beeState = BeeStateEnumerator.Dying;
                                enemyState.velocity *= .5f;
                                state.EntityManager.SetComponentData(target.ValueRW.target, enemyState);
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
                        !SystemAPI.Exists(target.ValueRO.target))
                    {
                        bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                    }
                    else
                    {
                        var isCarried = SystemAPI.IsComponentEnabled<ResourceCarried>(target.ValueRO.target);
                        if (isCarried)
                        {
                            bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                        }
                        else
                        {
                            var targetTransform = SystemAPI.GetAspectRO<TransformAspect>(target.ValueRO.target);
                            var delta = targetTransform.WorldPosition - transform.WorldPosition;
                            var sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                            if (sqrDist > config.grabDistance * config.grabDistance)
                            {
                                velocity += delta * (config.chaseForce * deltaTime / math.sqrt(sqrDist));
                            }
                            else
                            {
                                //Debug.Log($"Grabbing resource {target.ValueRO.targetPosition} - {transform.WorldPosition}, dist {sqrDist}");
                                var resource = SystemAPI.GetComponent<Resource>(target.ValueRO.target);
                                resource.ownerBee = entity;
                                SystemAPI.SetComponent(target.ValueRO.target, resource);
                                SystemAPI.SetComponentEnabled<ResourceCarried>(target.ValueRO.target, true);
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
                        if (target.IsValid && SystemAPI.HasComponent<Resource>(target.ValueRO.target))
                        {
                            var resource = SystemAPI.GetComponent<Resource>(target.ValueRO.target);
                            resource.ownerBee = Entity.Null;
                            SystemAPI.SetComponent(target.ValueRO.target, resource);
                            SystemAPI.SetComponentEnabled<ResourceCarried>(target.ValueRO.target, false);
                            SystemAPI.SetComponentEnabled<ResourceDropped>(target.ValueRO.target, true);
                        }

                        bee.ValueRW.beeState = BeeStateEnumerator.Idle;
                    }

                    break;
                }
                case BeeStateEnumerator.Dying:
                    SpawnParticles(config, ecb, transform.LocalPosition, 5);

                    entitiesToDestroy.Add(entity);
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

        // Destroying entities of all bees that have fallen to the floor
        foreach(Entity entity in entitiesToDestroy)
        {
            state.EntityManager.DestroyEntity(entity);
        }
    }

    private static void SpawnParticles(Config config, EntityCommandBuffer ecb, float3 position, int count)
    {
        var particles = new NativeArray<Entity>(count, Allocator.Temp);
        ecb.Instantiate(config.particlePrefab, particles);
        var color = new URPMaterialPropertyBaseColor { Value = new float4(0.8f, 0f, 0f, 1f) };
        var random = new Random();
        random.InitState((uint)position.GetHashCode());
        foreach (var particle in particles)
        {
            ecb.SetComponent(particle, color);
            var scale = random.NextFloat(.75f, 1f);
            ecb.SetComponent(particle, new LocalTransform
            {
                Position = position,
                Scale = scale,
                Rotation = quaternion.identity
            });
            ecb.SetComponent(particle, new Particle()
            {
                life = 1f,
                lifeTime = random.NextFloat(.75f, 2f),
                velocity = random.NextFloat3Direction() * 5f,
                size = scale
            });
            ecb.AddComponent(particle, new PostTransformScale()
            {
                Value = float3x3.Scale(scale)
            });
        }
    }
    
    bool CheckBoundingBox(float3 topRight, float3 bottomLeft, float3 beePosition)
    {
        return (topRight.x <= beePosition.x && beePosition.x <= bottomLeft.x
                                            && topRight.z <= beePosition.x && beePosition.x <= bottomLeft.z);
    }
}