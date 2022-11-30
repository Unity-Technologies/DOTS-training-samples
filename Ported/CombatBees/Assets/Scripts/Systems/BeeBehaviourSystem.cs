using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
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
        DynamicBuffer<AvailableResources> hive01AvailableResources;

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

            hive01AvailableResources = hiveAvailableResources;
        }

        var deltaTime = state.WorldUnmanaged.Time.DeltaTime;
        var rotationStrength = Mathf.Min(.5f * deltaTime, 1); // Some arbitrary value for speed of rotation

        foreach (var (transform, beeState, team, target, entity) in SystemAPI.Query<TransformAspect, RefRW<BeeState>, Team, RefRW<BeeTarget>>().WithEntityAccess())
        {
            bool jittering = true;

            switch (beeState.ValueRO.beeState)
            {
                case BeeStateEnumerator.Attacking:
                    if(target.ValueRW.target == Entity.Null)
                    {
                        var enemyBees = team.number == 0 ? hive0EnemyBees : hive1EnemyBees;

                        var randomIndex = UnityEngine.Random.Range(0, enemyBees.Length);
                        target.ValueRW.target = enemyBees[randomIndex].enemy;
                        target.ValueRW.targetPosition = enemyBees[randomIndex].enemyPosition;
                    }
                    else
                    {
                        var enemyBees = team.number == 0 ? hive0EnemyBees : hive1EnemyBees;
                        target.ValueRW.targetPosition = state.EntityManager.GetAspectRO<TransformAspect>(target.ValueRW.target).LocalPosition;

                        // Placeholder. TODO smooth rotation to face target, rather than snapping. The code below just results in jittering.
                        transform.LookAt(target.ValueRO.targetPosition);

                        /*var targetRotation = Quaternion.LookRotation(target.ValueRW.targetPosition);
                        transform.RotateLocal(Quaternion.Lerp(transform.LocalRotation, targetRotation, rotationStrength));
                        //transform.RotateLocal(Quaternion.Lerp(transform.LocalRotation, targetRotation, rotationStrength));*/

                        transform.LocalPosition += transform.Forward * deltaTime * 3f;
                    }
                    break;
                case BeeStateEnumerator.Gathering:
                    if (target.ValueRW.target != Entity.Null && SystemAPI.HasComponent<Resource>(target.ValueRW.target))
                    {
                        var targetResource = SystemAPI.GetComponent<LocalTransform>(target.ValueRW.target);
                        if (math.distancesq(transform.WorldPosition, targetResource.Position) < 0.5)
                        {
                            beeState.ValueRW.beeState = BeeStateEnumerator.CarryBack;
                            break;
                        }
                        
                        var resourcePosition = SystemAPI.GetComponent<LocalTransform>(target.ValueRW.target).Position;
                        var targetRotation = Quaternion.LookRotation(resourcePosition);
                        transform.LocalRotation = Quaternion.RotateTowards(transform.LocalRotation, targetRotation, 100);
                        transform.LocalPosition = math.lerp(transform.LocalPosition, resourcePosition, SystemAPI.Time.DeltaTime);
                        
                        break;
                    }
                    
                    float dist = float.MaxValue;

                    foreach (var (resourceTransform, resourceData, resourceEntity) in SystemAPI.Query<TransformAspect, RefRW<Resource>>().WithEntityAccess())
                    {
                        if (resourceData.ValueRW.ownerBee != Entity.Null)
                        {
                            // we probably want to check if the owner bee is friendly or not
                            // and set ourselves to attacking if yes. The resourceData.ownerBee
                            // would become the targetBee for the current bee
                            break;
                        }

                        var distToCurrent = math.distancesq(transform.WorldPosition, resourceTransform.Position);
                        if (distToCurrent < dist)
                        {
                            dist = distToCurrent;
                            resourceData.ValueRW.ownerBee = entity;
                            target.ValueRW.target = resourceEntity;
                        }
                    }
                    break;
                case BeeStateEnumerator.CarryBack:
                    foreach (var (hive, hiveTeam) in SystemAPI.Query<RefRO<Hive>, Team>())
                    {
                        if (hiveTeam.number == team.number)
                        {
                            var topRight = hive.ValueRO.boundsPosition + hive.ValueRO.boundsExtents;
                            var bottomLeft = hive.ValueRO.boundsPosition - hive.ValueRO.boundsExtents;
                            if (CheckBoundingBox(topRight, bottomLeft, transform.WorldPosition))
                            {
                                target.ValueRW.target = Entity.Null;
                                beeState.ValueRW.beeState = BeeStateEnumerator.Attacking;
                                break;
                            }
                            var hivePosition = hive.ValueRO.boundsPosition;
                            var targetRotation = Quaternion.LookRotation(hivePosition);
                            transform.LocalRotation = Quaternion.RotateTowards(transform.LocalRotation, targetRotation, 100);
                            transform.LocalPosition = math.lerp(transform.LocalPosition, hivePosition, SystemAPI.Time.DeltaTime);  
                        }
                    }
                    break;
                case BeeStateEnumerator.Dying:
                    jittering = false;

                    break;
            }

            if (jittering)
            {
                transform.LocalPosition += (float3)UnityEngine.Random.insideUnitSphere * (1f * deltaTime);
            }
        }
    }
    
    bool CheckBoundingBox(float3 topRight, float3 bottomLeft, float3 beePosition)
    {
        return (topRight.x <= beePosition.x && beePosition.x <= bottomLeft.x
                                            && topRight.z <= beePosition.x && beePosition.x <= bottomLeft.z);
    }
}