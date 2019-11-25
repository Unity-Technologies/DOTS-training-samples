using Unity.Burst;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CalculateVelocityAfterTransportingResourceSystem))]
    public class CalculatePositionAfterTransportingResourceSystem : JobComponentSystem
    {
        EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = _mapQuery.GetSingleton<Map>();
            return new Job
            {
                MapWidth = map.Width,
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, Velocity>
        {
            public float MapWidth;

            public void Execute(ref Position position, ref Velocity velocity)
            {
                float2 targetPosition = position.Value;
                float2 targetVelocity = velocity.Value;
                
                if (targetPosition.x + targetVelocity.x < 0f || targetPosition.x + targetVelocity.x > this.MapWidth)
                {
                    targetVelocity.x = -targetVelocity.x;
                }
                else
                {
                    targetPosition.x += targetVelocity.x;
                }
                if (targetPosition.y + targetVelocity.y < 0f || targetPosition.y + targetVelocity.y > this.MapWidth)
                {
                    targetVelocity.y = -targetVelocity.y;
                }
                else
                {
                    targetPosition.y += targetVelocity.y;
                }
                position.Value = targetPosition;
                velocity.Value = targetVelocity;
            }
        }
    }
}