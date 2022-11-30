using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct BeeBehaviourSystem : ISystem
{
    EntityQuery resourceQuery;
    ComponentLookup<Resource> resourceLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        resourceQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Resource>().Build(ref state);

        resourceLookup = state.GetComponentLookup<Resource>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        DynamicBuffer<EnemyBees> hive0EnemyBees = new DynamicBuffer<EnemyBees>();
        DynamicBuffer<EnemyBees> hive1EnemyBees = new DynamicBuffer<EnemyBees>();

        foreach (var (hiveEnemyBees, hiveAvailableResources, team) in SystemAPI.Query<DynamicBuffer<EnemyBees>, DynamicBuffer<AvailableResources>, Team>())
        {
            if(team.number == 0)
            {
                hive0EnemyBees = hiveEnemyBees;
            }
            else
            {
                hive1EnemyBees = hiveEnemyBees;
            }
        }

        var deltaTime = state.WorldUnmanaged.Time.DeltaTime;

        NativeList<Entity> entitiesToDestroy = new NativeList<Entity>(Allocator.TempJob);

        foreach (var (transform, beeState, team, target, entity) in SystemAPI.Query<TransformAspect, RefRW<BeeState>, Team, RefRW<BeeTarget>>().WithEntityAccess())
        {
            bool jittering = true;

            switch (beeState.ValueRO.beeState)
            {
                case BeeStateEnumerator.Attacking:
                    if (target.ValueRW.target == Entity.Null || !state.EntityManager.Exists(target.ValueRW.target))
                    {
                        var enemyBees = team.number == 0 ? hive0EnemyBees : hive1EnemyBees;
                        if(enemyBees.Length == 0) { break; }

                        var randomIndex = UnityEngine.Random.Range(0, enemyBees.Length);
                        target.ValueRW.target = enemyBees[randomIndex].enemy;
                        target.ValueRW.targetPosition = enemyBees[randomIndex].enemyPosition;
                    }
                    else
                    {
                        var enemyBees = team.number == 0 ? hive0EnemyBees : hive1EnemyBees;
                        target.ValueRW.targetPosition = state.EntityManager.GetAspectRO<TransformAspect>(target.ValueRW.target).LocalPosition;

                        var targetPosition = target.ValueRO.targetPosition;
                        var targetRotation = Quaternion.LookRotation(targetPosition - transform.LocalPosition);
                        transform.LocalRotation = Quaternion.RotateTowards(transform.LocalRotation, targetRotation, 10); // last value is arbitrary. Just found something that looks the nicest.
                        transform.LocalPosition += transform.Forward * deltaTime * 7f;

                        float distanceToTarget = Vector3.Distance(transform.LocalPosition, targetPosition);

                        if (distanceToTarget < 3f)
                        {
                            transform.LocalPosition += transform.Forward * deltaTime * 7f;
                            if (distanceToTarget < 1f)
                            {
                                BeeState dyingState = new BeeState() { beeState = BeeStateEnumerator.Dying };
                                state.EntityManager.SetComponentData<BeeState>(target.ValueRW.target, dyingState);
                                target.ValueRW.target = Entity.Null;
                            }
                        }
                    }
                    break;
                case BeeStateEnumerator.Gathering:
                    if (target.ValueRW.target != Entity.Null)
                    {
                        if (SystemAPI.HasComponent<Resource>(target.ValueRW.target))
                        {
                            var targetResource = SystemAPI.GetComponent<LocalTransform>(target.ValueRW.target);
                            if (math.distance(transform.LocalPosition, targetResource.Position) < 0.5)
                            {
                                beeState.ValueRW.beeState = BeeStateEnumerator.CarryBack;
                                SystemAPI.SetComponentEnabled<ResourceCarried>(target.ValueRW.target, true);
                                SystemAPI.SetComponentEnabled<ResourceDropped>(target.ValueRW.target, false);
                            }
                            else
                            {
                                var resourcePosition = targetResource.Position;
                                var targetRotation = Quaternion.LookRotation(resourcePosition);
                                transform.LocalRotation = Quaternion.RotateTowards(transform.LocalRotation, targetRotation, 100);
                                transform.LocalPosition = math.lerp(transform.LocalPosition, resourcePosition, SystemAPI.Time.DeltaTime);
                            }
                        }
                    }
                    else
                    {
                        float minDist = float.MaxValue;

                        foreach (var (resourceTransform, resource, resourceDropped, resourceEntity) in SystemAPI.Query<TransformAspect, RefRW<Resource>,ResourceDropped>().WithEntityAccess())
                        {
                            if (resource.ValueRW.ownerBee != Entity.Null)
                                continue;
                            
                            var distToCurrent = math.distance(transform.WorldPosition, resourceTransform.Position);
                            
                            if (minDist > distToCurrent)
                                minDist = distToCurrent;
                        }

                        // If no close target was found
                        if (Math.Abs(minDist - float.MaxValue) < math.EPSILON)
                            break;
                        
                        // :) idk how to access a RW component differently (:
                        foreach (var (resourceTransform, resourceData, resourceEntity) in SystemAPI.Query<TransformAspect, RefRW<Resource>>().WithEntityAccess())
                        {
                            var distToCurrent = math.distance(transform.LocalPosition, resourceTransform.Position);
                            if (math.abs(minDist - distToCurrent) > math.EPSILON)
                                continue;

                            var odds = 0.5f;
                            var roll = UnityEngine.Random.Range(0f, 1f);

                            if (odds < roll)
                                break;
                            
                            resourceData.ValueRW.ownerBee = entity;
                            target.ValueRW.target = resourceEntity;
                            break;
                        }
                    }
                    break;
                case BeeStateEnumerator.CarryBack:
                    foreach (var (hive, hiveTeam) in SystemAPI.Query<RefRO<Hive>, Team>())
                    {
                        if (hiveTeam.number != team.number) 
                            continue;
                        
                        if (math.distance(hive.ValueRO.boundsPosition, transform.LocalPosition) < 0.5f)
                        {
                            SystemAPI.SetComponentEnabled<ResourceCarried>(target.ValueRW.target, false);
                            SystemAPI.SetComponentEnabled<ResourceDropped>(target.ValueRW.target, false);
                            SystemAPI.SetComponentEnabled<ResourceHiveReached>(target.ValueRW.target, true);
                            
                            target.ValueRW.target = Entity.Null;
                            beeState.ValueRW.beeState = BeeStateEnumerator.Gathering;
                        }
                        else
                        {
                            var hivePosition = hive.ValueRO.boundsPosition;
                            var targetRotation = Quaternion.LookRotation(hivePosition);
                            transform.LocalRotation = Quaternion.RotateTowards(transform.LocalRotation, targetRotation, 100);
                            transform.LocalPosition = math.lerp(transform.LocalPosition, hivePosition, SystemAPI.Time.DeltaTime);  
                        }
                    }
                    break;
                case BeeStateEnumerator.Dying:
                    jittering = false;

                    float floorY = -5f;
                    float gravity = 0.1f;

                    transform.LocalPosition = GetFallingPos(transform.LocalPosition, floorY, gravity);

                    if (transform.LocalPosition.y <= floorY)
                    {
                        var config = SystemAPI.GetSingleton<Config>();
                        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                        SpawnParticles(config, ecb, transform.LocalPosition, 5);
                        
                        if (target.ValueRW.target != Entity.Null && SystemAPI.HasComponent<Resource>(target.ValueRW.target))
                        {
                            SystemAPI.SetComponentEnabled<ResourceCarried>(target.ValueRW.target, false);
                            SystemAPI.SetComponentEnabled<ResourceDropped>(target.ValueRW.target, true);
                            
                            // :) idk how to access a RW component differently (:
                            foreach (var (resource, resourceEntity) in SystemAPI.Query<RefRW<Resource>>().WithEntityAccess())
                            {
                                if (resourceEntity != target.ValueRO.target)
                                    continue;
                                
                                resource.ValueRW.ownerBee = Entity.Null;
                                break;
                            }
                            
                        }
                        entitiesToDestroy.Add(entity);
                    }

                    break;
            }

            if (jittering)
            {
                transform.LocalPosition += (float3)UnityEngine.Random.insideUnitSphere * (1f * deltaTime);
            }
        }

        // Destroying entities of all bees that have fallen to the floor
        foreach(Entity entity in entitiesToDestroy)
        {
            state.EntityManager.DestroyEntity(entity);
        }
    }

    float3 GetFallingPos(float3 position, float floor, float gravity)
    {
        if (position.y > floor)
        {
            position = new float3(position.x, position.y - gravity /*fake gravity for now*/, position.z);
        }

        return position;
    }

    private static void SpawnParticles(Config config, EntityCommandBuffer ecb, float3 position, int count)
    {
        var particles = new NativeArray<Entity>(count, Allocator.Temp);
        ecb.Instantiate(config.particlePrefab, particles);
        var color = new URPMaterialPropertyBaseColor { Value = new float4(0.8f, 0f, 0f, 1f) };
        var random = new Unity.Mathematics.Random();
        random.InitState((uint)position.GetHashCode());
        foreach (var particle in particles)
        {
            ecb.SetComponent(particle, color);
            var scale = random.NextFloat(.25f, .4f);
            ecb.SetComponent(particle, new LocalTransform
            {
                Position = position,
                Scale = scale,
                Rotation = quaternion.identity
            });
            ecb.SetComponent(particle, new Particle()
            {
                life = 1f,
                lifeTime = random.NextFloat(.75f, 1f),
                velocity = random.NextFloat3Direction() * 5f,
                size = scale
            });
            ecb.AddComponent(particle, new PostTransformScale()
            {
                Value = float3x3.Scale(scale)
            });
        }
    }
}