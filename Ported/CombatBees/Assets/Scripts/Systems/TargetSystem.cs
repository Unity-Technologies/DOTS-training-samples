using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class TargetSystem : SystemBase
    {
        const float aggression = 0.5f; // Move to be a configurable parameter

        EntityQuery[] teamTargets;
        EntityQuery resourceTargets;

        protected override void OnCreate()
        {
            teamTargets = new EntityQuery[2]
            {
                    GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team>()),
                    GetEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Team>())
            };
            teamTargets[0].SetSharedComponentFilter(new Team { TeamId = 0 });
            teamTargets[1].SetSharedComponentFilter(new Team { TeamId = 1 });


            //resourceTargets = GetEntityQuery(ComponentType.ReadOnly<Resource>());
        }

        protected override void OnUpdate()
        {
            var team0Targets = teamTargets[0].ToEntityArray(Allocator.TempJob);
            var team1Targets = teamTargets[1].ToEntityArray(Allocator.TempJob);
            var team0Positions = teamTargets[0].ToComponentDataArray<Translation>(Allocator.TempJob);
            var team1Positions = teamTargets[1].ToComponentDataArray<Translation>(Allocator.TempJob);


            //var resources = resourceTargets.ToEntityArray(Allocator.Temp);

            var globalSystemVersion = GlobalSystemVersion;


            Entities
                .WithReadOnly(team0Targets)
                .WithReadOnly(team0Positions)
                .WithDisposeOnCompletion(team0Targets)
                .WithDisposeOnCompletion(team0Positions)
                .WithSharedComponentFilter(new Team { TeamId = 1 })
                .ForEach((Entity entity, ref Target target) =>
                {
                    if (target.TargetEntity == null)
                    {
                        EntityUpdate(globalSystemVersion, in team0Targets, in team0Positions, entity, ref target);
                    }
                    else if (target.Type == Target.TargetType.Enemy && HasComponent<Translation>(target.TargetEntity))
                    {
                        float3 newPos = GetComponent<Translation>(target.TargetEntity).Value;
                        UpdateEnemyPosition(ref target, newPos);
                    }
                }).ScheduleParallel();

            Entities
                .WithReadOnly(team1Targets)
                .WithReadOnly(team1Positions)
                .WithDisposeOnCompletion(team1Targets)
                .WithDisposeOnCompletion(team1Positions)
                .WithSharedComponentFilter(new Team { TeamId = 0 })
                .ForEach((Entity entity, ref Target target) =>
                {
                    if (target.TargetEntity == null)
                    {
                        EntityUpdate(globalSystemVersion, in team1Targets, in team1Positions, entity, ref target);
                    }
                    else if (target.Type == Target.TargetType.Enemy && HasComponent<Translation>(target.TargetEntity))
                    {
                        float3 newPos = GetComponent<Translation>(target.TargetEntity).Value;
                        UpdateEnemyPosition(ref target, newPos);
                    }
                }).ScheduleParallel();

        }

        private static void EntityUpdate(uint globalSystemVersion, in NativeArray<Entity> attackables, in NativeArray<Translation> positions, Entity entity, ref Target target)
        {
            var random = Random.CreateFromIndex(globalSystemVersion ^ (uint)entity.Index);

            // Relying on this check stops filtering being effective, but otherwise there'd be a lot of structural churn
            
            if (random.NextFloat() < aggression)
            {
                int attackableIndex = random.NextInt(attackables.Length - 1);
                target = new Target
                {
                    TargetEntity = attackables[attackableIndex],
                    Position = positions[attackableIndex].Value,
                    Type = Target.TargetType.Enemy
                };
            }
            else
            {
                //int resourceIndex = random.NextInt(attackables.Length - 1);
                //target = new Target
                //{
                //    TargetEntity = resources[resourceIndex]
                //});
            }
        }

        private static void UpdateEnemyPosition(ref Target target, float3 position)
        {
            target = new Target
            {
                TargetEntity = target.TargetEntity,
                Position = position,
                Type = Target.TargetType.Enemy
            };
        }
    }
}