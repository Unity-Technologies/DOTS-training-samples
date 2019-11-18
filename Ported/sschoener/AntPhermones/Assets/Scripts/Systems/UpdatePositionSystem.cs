using System;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ResourceCarrySystem))]
public class UpdatePositionSystem : JobComponentSystem
{
    EntityQuery m_MapQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        return new Job
        {
            MapSize = map.MapSize,
        }.Schedule(this, inputDeps);
    }

    struct Job : IJobForEach<PositionComponent, VelocityComponent>
    {
        public float MapSize;

        public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
        {
            var p = position.Value;
            var v = velocity.Value;
            if (p.x + v.x < 0f || p.x + v.x > MapSize)
            {
                v.x = -v.x;
            }
            else
            {
                p.x += v.x;
            }
            if (p.y + v.y < 0f || p.y + v.y > MapSize)
            {
                v.y = -v.y;
            }
            else
            {
                p.y += v.y;
            }
            position.Value = p;
            velocity.Value = v;
        }
    }
}
