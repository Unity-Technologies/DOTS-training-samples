using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct BeeBehaviourSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
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
                        if (distanceToTarget < 1f)
                        {
                            BeeState dyingState = new BeeState() { beeState = BeeStateEnumerator.Dying };
                            state.EntityManager.SetComponentData<BeeState>(target.ValueRW.target, dyingState);
                            target.ValueRW.target = Entity.Null;
                        }
                    }
                    break;
                case BeeStateEnumerator.Gathering:
                    //
                    break;
                case BeeStateEnumerator.CarryBack:
                    //
                    break;
                case BeeStateEnumerator.Dying:

                    jittering = false;

                    float floorY = -5f;
                    float gravity = 0.1f;

                    transform.LocalPosition = GetFallingPos(transform.LocalPosition, floorY, gravity);

                    if(transform.LocalPosition.y <= floorY)
                    {
                        // Blood particle logic

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
}