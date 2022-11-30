using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct HiveSystem : ISystem
{
    NativeList<EnemyBees> hive0EnemyBees;
    NativeList<EnemyBees> hive1EnemyBees;
    NativeList<AvailableResources> hive01AvailableResources;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        hive0EnemyBees = new NativeList<EnemyBees>(Allocator.Persistent);
        hive1EnemyBees = new NativeList<EnemyBees>(Allocator.Persistent);
        hive01AvailableResources = new NativeList<AvailableResources>(Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        hive0EnemyBees.Dispose();
        hive1EnemyBees.Dispose();
        hive01AvailableResources.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Workaround solution to move the logic outside of nested ForEach Queries
        hive0EnemyBees.Clear();
        hive1EnemyBees.Clear();
        hive01AvailableResources.Clear();

        Team hive0EnemyTeam = new Team { number = 1 };
        Team hive1EnemyTeam = new Team { number = 0 };

        foreach (var (transform, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeState>().WithEntityAccess().WithSharedComponentFilter(hive0EnemyTeam))
        {
            hive0EnemyBees.Add(new EnemyBees { enemy = beeEntity, enemyPosition = transform.LocalPosition });
        }

        foreach (var (transform, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeState>().WithEntityAccess().WithSharedComponentFilter(hive1EnemyTeam))
        {
            hive1EnemyBees.Add(new EnemyBees { enemy = beeEntity, enemyPosition = transform.LocalPosition });
            Debug.Log("B");
        }

        // TODO similar foreach to populate the hive01AvailableResources buffer

        if (!hive0EnemyBees.IsEmpty || !hive1EnemyBees.IsEmpty)
        {
            foreach (var (hiveEnemyBees, hiveAvailableResources, team) in SystemAPI.Query<DynamicBuffer<EnemyBees>, DynamicBuffer<AvailableResources>, Team>())
            {
                hiveEnemyBees.Clear();
                hiveAvailableResources.Clear();

                if (team.number == 0)
                {
                    hiveEnemyBees.CopyFrom(hive0EnemyBees.AsArray());
                }
                else
                {
                    hiveEnemyBees.CopyFrom(hive1EnemyBees.AsArray());
                }

                //hiveAvailableResources.CopyFrom(hive01AvailableResources);
            }
        }

        //============================================================================================

        // Previous (broken) nested ForEach Query solution

        /*foreach (var (enemyBees, resources, hiveTeam, hiveEntity) in SystemAPI.Query<DynamicBuffer<EnemyBees>, DynamicBuffer<AvailableResources>, Team>().WithEntityAccess())
        {
            uint whichTeamIsEnemy = (uint)(hiveTeam.number == 0 ? 1 : 0);
            Team enemyTeam = new Team { number = whichTeamIsEnemy };

            enemyBees.Clear();
            resources.Clear();

            // Issue: Only one of the SharedComponents' buffers is accessed, while it should be both. So only one hive is populated with references to enemy bees.
            // Nested foreach support has only recently been introduced, so this might not be an issue with the code. Regardless, we'll need a different solution.
            foreach (var (transform, beeEntity) in SystemAPI.Query<TransformAspect>().WithAll<BeeState>().WithEntityAccess().WithSharedComponentFilter(enemyTeam))
            {
                enemyBees.Add(new EnemyBees { enemy = beeEntity, enemyPosition = transform.LocalPosition });
            }

            // For resources, pretty much same as above, but without shared components
        }*/
    }
}