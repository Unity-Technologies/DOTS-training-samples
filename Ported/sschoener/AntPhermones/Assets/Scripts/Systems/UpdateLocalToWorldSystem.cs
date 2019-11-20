using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateLocalToWorldSystem : JobComponentSystem
{
    EntityQuery m_AntSettingsQuery;
    EntityQuery m_MapSettingsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<AntRenderSettingsComponent>());
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ants = m_AntSettingsQuery.GetSingleton<AntRenderSettingsComponent>();
        var map = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>(); 
        return new Job
        {
            Scale = ants.Scale,
            MapSize =  map.MapSize
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobForEach<FacingAngleComponent, PositionComponent, LocalToWorldComponent>
    {
        public float3 Scale;
        public int MapSize;

        public void Execute([ReadOnly] ref FacingAngleComponent angle, [ReadOnly] ref PositionComponent position, [WriteOnly] ref LocalToWorldComponent localToWorld)
        {
            localToWorld.Value = float4x4.TRS(
                new float3(position.Value / MapSize, 0),
                quaternion.Euler(new float3(0, 0, angle.Value)),
                Scale);
        }
    }
}
