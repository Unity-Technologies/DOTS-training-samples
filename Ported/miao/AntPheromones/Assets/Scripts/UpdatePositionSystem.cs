using Unity.Mathematics;

namespace AntPheromones_ECS
{
    using System;
    using Unity.Entities;
    using Unity.Jobs;

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class UpdatePositionSystem : JobComponentSystem
    {
        EntityQuery m_MapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = m_MapQuery.GetSingleton<Map>();
            return new Job
            {
                MapWidth = map.Width,
            }.Schedule(this, inputDeps);
        }

        struct Job : IJobForEach<Position, Velocity>
        {
            public float MapWidth;

            public void Execute(ref Position position, ref Velocity velocity)
            {
                float2 targetPosition = position.Value;
                float2 targetVelocity = velocity.Value;
                
                if (targetPosition.x + targetVelocity.x < 0f || targetPosition.x + targetVelocity.x > MapWidth)
                {
                    targetVelocity.x = -targetVelocity.x;
                }
                else
                {
                    targetPosition.x += targetVelocity.x;
                }
                if (targetPosition.y + targetVelocity.y < 0f || targetPosition.y + targetVelocity.y > MapWidth)
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