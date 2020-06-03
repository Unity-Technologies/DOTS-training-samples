using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DefaultNamespace
{
    public class Scooper : SystemBase
    {
        private EntityQuery m_WaterSourceQuery;

        protected override void OnCreate()
        {
            m_WaterSourceQuery = GetEntityQuery(
                new EntityQueryDesc
                {
                    All = new []{ComponentType.ReadOnly<WaterSource>() }
                });
        }

        protected override void OnUpdate()
        {
            var waterEntities = m_WaterSourceQuery.ToEntityArrayAsync(Allocator.TempJob, out var fetchWaterEntitiesJob);
            
            var translationComponent = GetComponentDataFromEntity<LocalToWorld>();
            var chainComponent = GetComponentDataFromEntity<Chain>();
            
            Dependency = Entities.ForEach((ref ScooperState state, ref TargetPosition targetPosition, ref TargetWaterSource targetWaterSource, in Translation position, in Agent agent)
                =>
            {
                switch (state.State)
                {
                    case EScooperState.FindWater:
                        // Find closest water to my position
                        var nearestWater = FindNearestWater(translationComponent, waterEntities, position);
                        var nearestWaterPosition = translationComponent[nearestWater];

                        var chain = chainComponent[agent.MyChain];
                        chain.ChainStartPosition = nearestWaterPosition.Position;
                        chainComponent[agent.MyChain] = chain;

                        targetWaterSource.Target = nearestWater;
                        targetPosition.Target = nearestWaterPosition.Position;
                        break;
                }
            }).WithoutBurst().WithNativeDisableParallelForRestriction(chainComponent).WithReadOnly(translationComponent).ScheduleParallel(fetchWaterEntitiesJob);
        }

        private static Entity FindNearestWater(ComponentDataFromEntity<LocalToWorld> translationComponent,
            NativeArray<Entity> waterEntities, in Translation position)
        {
            Entity nearestEntity = default;
            float nearestDistanceSq = float.MaxValue;
            
            for (int i = 0; i < waterEntities.Length; ++i)
            {
                var waterEntity = waterEntities[i];
                var waterPosition = translationComponent[waterEntity];
                var distanceSq = math.distancesq(waterPosition.Position, position.Value);
                if (distanceSq < nearestDistanceSq)
                {
                    nearestDistanceSq = distanceSq;
                    nearestEntity = waterEntity;
                }
            }

            return nearestEntity;
        }
    }
}