using Unity.Burst;
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
                    //
                    break;
                case BeeStateEnumerator.CarryBack:
                    //
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
}