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
                    GetEntityQuery(ComponentType.ReadOnly<Attackable>(), ComponentType.ReadOnly<Team>()),
                    GetEntityQuery(ComponentType.ReadOnly<Attackable>(), ComponentType.ReadOnly<Team>())
            };
            teamTargets[0].SetSharedComponentFilter(new Team { TeamId = 0 });
            teamTargets[1].SetSharedComponentFilter(new Team { TeamId = 1 });


            //resourceTargets = GetEntityQuery(ComponentType.ReadOnly<Resource>());
        }

        protected override void OnUpdate()
        {
            var team0Targets = teamTargets[0].ToEntityArray(Allocator.TempJob);
            var team1Targets = teamTargets[1].ToEntityArray(Allocator.TempJob);


            //var resources = resourceTargets.ToEntityArray(Allocator.Temp);

            var globalSystemVersion = GlobalSystemVersion;


            Entities
                .WithReadOnly(team0Targets)
                .WithDisposeOnCompletion(team0Targets)
                .WithSharedComponentFilter(new Team { TeamId = 1 })
                .ForEach((Entity entity, ref Target target) =>
                {
                    EntityUpdate(globalSystemVersion, team0Targets, entity, ref target);
                }).ScheduleParallel();

            Entities
                .WithReadOnly(team1Targets)
                .WithDisposeOnCompletion(team1Targets)
                .WithSharedComponentFilter(new Team { TeamId = 0 })
                .ForEach((Entity entity, ref Target target) =>
                {
                    EntityUpdate(globalSystemVersion, team1Targets, entity, ref target);
                }).ScheduleParallel();

        }

        private static void EntityUpdate(uint globalSystemVersion, NativeArray<Entity> attackables, Entity entity, ref Target target)
        {
            var random = Random.CreateFromIndex(globalSystemVersion ^ (uint)entity.Index);

            // Relying on this check stops filtering being effective, but otherwise there'd be a lot of structural churn
            if (target.TargetEntity == null)
            {
                if (random.NextFloat() < aggression)
                {
                    int attackableIndex = random.NextInt(attackables.Length - 1);
                    target = new Target
                    {
                        TargetEntity = attackables[attackableIndex],
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
        }
    }
}