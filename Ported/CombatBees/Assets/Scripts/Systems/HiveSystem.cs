using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct HiveSystem : ISystem
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
        foreach (var (enemyBees, resources, hiveTeam, hiveEntity) in SystemAPI.Query<DynamicBuffer<EnemyBees>, DynamicBuffer<AvailableResources>, Team>().WithEntityAccess())
        {
            uint whichTeamIsEnemy = (uint)(hiveTeam.number == 0 ? 1 : 0);
            Team enemyTeam = new Team { number = whichTeamIsEnemy };

            enemyBees.Clear();
            resources.Clear();

            // Issue: Only one of the SharedComponents' buffers is accessed, while it should be both. So only one hive is populated with references to enemy bees.
            // Nested foreach support has only recently been introduced, so this might not be an issue with the code. Regardless, we'll need a different solution.
            foreach (var (beeState, transform, beeEntity) in SystemAPI.Query<BeeState, TransformAspect>().WithEntityAccess().WithSharedComponentFilter(enemyTeam))
            {
                enemyBees.Add(new EnemyBees { enemy = beeEntity, enemyPosition = transform.LocalPosition });
            }

            // For resources, pretty much same as above, but without shared components
        }
    }
}