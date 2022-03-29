using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial class TargetSystem : SystemBase
    {
        const float aggression = 0.5f; // Move to be a configurable parameter

        EntityQuery enemyTargets;
        EntityQuery resourceTargets;

        protected override void OnCreate()
        {
            enemyTargets = GetEntityQuery(ComponentType.ReadOnly<Attackable>());
            //resourceTargets = GetEntityQuery(ComponentType.ReadOnly<Resource>());
        }

        protected override void OnUpdate()
        {
            var random = new Random(1234);

            var attackables = enemyTargets.ToEntityArray(Allocator.TempJob);
            //var resources = resourceTargets.ToEntityArray(Allocator.Temp);

            Entities
                .ForEach((int entityInQueryIndex, ref Target target) =>
                {
                    // Relying on this check stops filtering being effective, but otherwise there'd be a lot of structural churn
                    //if (target.TargetEntity == null)
                    //{
                    //    if (random.NextFloat() < aggression)
                    //    {
                    //        int attackableIndex = entityInQueryIndex;
                    //        while (attackableIndex != entityInQueryIndex)
                    //        {
                    //            attackableIndex = random.NextInt(attackables.Length - 1);
                    //        }
                    //        target = new Target
                    //        {
                    //            TargetEntity = attackables[attackableIndex],
                    //            Type = Target.TargetType.Enemy
                    //        };
                    //    }
                    //    else
                    //    {
                    //        //int resourceIndex = random.NextInt(attackables.Length - 1);
                    //        //target = new Target
                    //        //{
                    //        //    TargetEntity = resources[resourceIndex]
                    //        //});
                    //    }
                    //}
                }).ScheduleParallel();

            attackables.Dispose();
        }
    }
}