using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AssignTargetSystem : SystemBase
{
    EntityQuery m_TeamOneQuery;
    EntityQuery m_TeamTwoQuery;
    EntityQuery m_ResourceQuery;

    Random m_Random;

    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ResourceQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<ResourceEntity>()
            }
        });

        m_TeamOneQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<Target>()
            }
        });

        m_TeamTwoQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamTwo>(),
                ComponentType.ReadOnly<Target>()
            }
        });


        m_Random = new Random(0x5716318);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var random = m_Random;
        var resourceEntities = m_ResourceQuery.ToEntityArray(Allocator.TempJob);
        var teamOneEntities = m_TeamOneQuery.ToEntityArray(Allocator.TempJob);
        var teamTwoEntities = m_TeamTwoQuery.ToEntityArray(Allocator.TempJob);


        //Dependency = JobHandle.CombineDependencies()
        //Dependency = JobHandle.CombineDependencies(Dependency, teamOneEntitiesHandle);
        //Dependency = JobHandle.CombineDependencies(Dependency, teamTwoEntitiesHandle);

        foreach (Entity e in teamOneEntities)
        {
            ComponentDataFromEntity<Target> myTypeFromEntity = GetComponentDataFromEntity<Target>(true);
            Target target = myTypeFromEntity[e];
            if (target.EnemyTarget == Entity.Null || target.ResourceTarget == Entity.Null)
            {

                var aggression = random.NextFloat(0, 1);
                if (aggression < 0.5f)
                {
                    target.EnemyTarget = teamTwoEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
                }
                else
                {
                    //var resourceEntities = m_ResourceQuery.ToEntityArray(Allocator.TempJob);
                    target.ResourceTarget = resourceEntities[random.NextInt(0, resourceEntities.Length - 1)];
                }
            }
            else if (target.EnemyTarget != null)
            {

            }
        }
        //Entities
        //    .ForEach((ref Target target) =>
        //    {
        //        if (target.EnemyTarget == Entity.Null || target.ResourceTarget == Entity.Null)
        //        {

        //            var aggression = random.NextFloat(0, 1);
        //            if (aggression < 0.5f)
        //            {
        //                target.EnemyTarget = teamTwoEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
        //            }
        //            else
        //            {
        //                //var resourceEntities = m_ResourceQuery.ToEntityArray(Allocator.TempJob);
        //                target.ResourceTarget = resourceEntities[random.NextInt(0, resourceEntities.Length - 1)];
        //            }
        //        }
        //        else if (target.EnemyTarget != null)
        //        {

        //        }

        //    }).Schedule();

        //Entities
        //    .WithDeallocateOnJobCompletion(teamOneEntities)
        //    .WithDeallocateOnJobCompletion(teamTwoEntities)
        //    .ForEach((ref Velocity velocity, in TeamTwo team, in Target target) =>
        //    {
        //        if (target.EnemyTarget == null || target.ResourceTarget == null)
        //        {
        //            var randomAttack = teamOneEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
        //        }

        //    }).Schedule();

        //m_ECBSystem.AddJobHandleForProducer(Dependency);

        m_Random = random;
    }
}
