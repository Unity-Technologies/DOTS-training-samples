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
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                MapWidth = this._mapQuery.GetSingleton<Map>().Width,
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, Velocity>
        {
            public float MapWidth;

            public void Execute(ref Position position, ref Velocity velocity)
            {
                if (position.Value.x + velocity.Value.x < 0f || position.Value.x + velocity.Value.x > this.MapWidth)
                {
                    velocity.Value.x = -velocity.Value.x;
                }
                else
                {
                    position.Value.x += velocity.Value.x;
                }
                if (position.Value.y + velocity.Value.y < 0f || position.Value.y + velocity.Value.y > this.MapWidth)
                {
                    velocity.Value.y = -velocity.Value.y;
                }
                else
                {
                    position.Value.y += velocity.Value.y;
                }
            }
        }
    }
}