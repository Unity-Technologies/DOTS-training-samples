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

            foreach (var (transform, beeState, beeEntity) in SystemAPI.Query<TransformAspect, RefRO<BeeState>>().WithEntityAccess().WithSharedComponentFilter(enemyTeam))
            {
                if (beeState.ValueRO.beeState != BeeStateEnumerator.Dying)
                {
                    enemyBees.Add(new EnemyBees { enemy = beeEntity, enemyPosition = transform.WorldPosition });
                }
            }

            // For resources, pretty much same as above, but without shared components
            foreach (var (transform, resource, entity) in SystemAPI.Query<TransformAspect, RefRO<Resource>>().WithNone<ResourceCarried, ResourceDropped>().WithEntityAccess())
            {
                resources.Add(new AvailableResources()
                {
                    resource = entity,
                    resourcePosition = transform.WorldPosition
                });
            }
        }
    }
}