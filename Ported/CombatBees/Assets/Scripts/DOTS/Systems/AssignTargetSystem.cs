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
        m_TeamOneQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<Target>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<DespawnTimer>()
            }
        });

        m_TeamTwoQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<TeamTwo>(),
                ComponentType.ReadOnly<Target>()
            },
            None = new[]
            {
                ComponentType.ReadOnly<DespawnTimer>()
            }
        });

        m_ResourceQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<ResourceEntity>()
            }
        });

        m_Random = new Random(0x5716318);

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var random = m_Random;

        var teamOneEntities = m_TeamOneQuery.ToEntityArrayAsync(Allocator.TempJob, out var teamOneEntitiesHandle);
        var teamTwoEntities = m_TeamTwoQuery.ToEntityArrayAsync(Allocator.TempJob, out var teamTwoEntitiesHandle);
        var resourceEntities = m_ResourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var resourceEntitiesHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, teamOneEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, teamTwoEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, resourceEntitiesHandle);

        Entities
            .WithNone<DespawnTimer>()
            .WithDeallocateOnJobCompletion(teamTwoEntities)
            .ForEach((ref Target target, in TeamOne team) =>
            {
                if (target.EnemyTarget == Entity.Null && target.ResourceTarget == Entity.Null)
                {
                    var aggression = random.NextFloat(0, 1);
                    if (aggression < 0.5f)
                    {
                        if (teamTwoEntities.Length > 0)
                            target.EnemyTarget = teamTwoEntities[random.NextInt(0, teamTwoEntities.Length - 1)];
                    }
                    else
                    {
                        if (resourceEntities.Length > 0)
                        {
                            target.ResourceTarget = resourceEntities[random.NextInt(0, resourceEntities.Length - 1)];
                            if (HasComponent<Gravity>(target.ResourceTarget))
                            {
                                target.ResourceTarget = Entity.Null;
                            }
                        }
                    }
                }
                else if (target.EnemyTarget != Entity.Null)
                {
                    if (HasComponent<DespawnTimer>(target.EnemyTarget))
                    {
                        target.EnemyTarget = Entity.Null;
                    }
                }
                else if (target.ResourceTarget != Entity.Null)
                {
                    if (HasComponent<DespawnTimer>(target.ResourceTarget))
                    {
                        target.ResourceTarget = Entity.Null;
                    }
                }

            }).Schedule();

        Entities
            .WithNone<DespawnTimer>()
            .WithDeallocateOnJobCompletion(teamOneEntities)
            .WithDeallocateOnJobCompletion(resourceEntities)
            .ForEach((ref Target target, in TeamTwo team) =>
            {
                if (target.EnemyTarget == Entity.Null && target.ResourceTarget == Entity.Null)
                {
                    var aggression = random.NextFloat(0, 1);
                    if (aggression < 0.5f)
                    {
                        if (teamOneEntities.Length > 0)
                            target.EnemyTarget = teamOneEntities[random.NextInt(0, teamOneEntities.Length - 1)];
                    }
                    else
                    {
                        if (resourceEntities.Length > 0)
                        {
                            target.ResourceTarget = resourceEntities[random.NextInt(0, resourceEntities.Length - 1)];
                            if (HasComponent<Gravity>(target.ResourceTarget))
                            {
                                target.ResourceTarget = Entity.Null;
                            }
                        }
                    }
                }
                else if (target.EnemyTarget != Entity.Null)
                {
                    if (HasComponent<DespawnTimer>(target.EnemyTarget))
                    {
                        target.EnemyTarget = Entity.Null;
                    }
                }
                else if (target.ResourceTarget != Entity.Null)
                {
                    if (HasComponent<DespawnTimer>(target.ResourceTarget))
                    {
                        target.ResourceTarget = Entity.Null;
                    }
                }

            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);

        m_Random = random;
    }
}
