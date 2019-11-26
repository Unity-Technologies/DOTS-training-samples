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
        var scale = ants.Scale;
        var mapSize = map.MapSize;
        return Entities.ForEach((ref LocalToWorldComponent localToWorld, in FacingAngleComponent angle, in PositionComponent position) =>
        {
            localToWorld.Value = float4x4.TRS(
                new float3(position.Value / mapSize, 0),
                quaternion.Euler(new float3(0, 0, angle.Value)),
                scale);
        }).Schedule(inputDeps);
    }
}
