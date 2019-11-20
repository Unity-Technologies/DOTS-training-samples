using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ComputeVelocitySystem))]
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
            MapSize = map.MapSize
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobForEach<PositionComponent, VelocityComponent>
    {
        public float MapSize;

        public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
        {
            var p = position.Value;
            var v = velocity.Value;
            var newP = p + v;
            if (newP.x < 0f || newP.x > MapSize)
            {
                v.x = -v.x;
            }
            else
            {
                p.x = newP.x;
            }
            if (newP.y < 0f || newP.y > MapSize)
            {
                v.y = -v.y;
            }
            else
            {
                p.y = newP.y;
            }
            position.Value = p;
            velocity.Value = v;
        }
    }
}
