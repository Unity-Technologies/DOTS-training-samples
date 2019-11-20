using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

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
            var newP = position.Value + velocity.Value;
            var mask = (float2)((newP < 0) | (newP > MapSize));
            position.Value += (1 - mask) * velocity.Value;
            velocity.Value -= 2 * velocity.Value * mask;
        }
    }
}
