using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class FireSpotLookupSystem : SystemBase
{
    /// <summary>
    ///     The delay between each lookup
    /// </summary>
    const double k_Delay = 5;

    /// <summary>
    ///     Stores the date of the last check for delaying purpose.
    /// </summary>
    double? m_LastCheck;

    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;

        if (m_LastCheck.HasValue && (time - m_LastCheck.Value < k_Delay))
        {
            return;
        }
        m_LastCheck = time;

        var heatmapData = GetSingleton<HeatMapData>();
        var heatmap = BucketBrigadeUtility.GetHeatmapBuffer(this);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var teamReformBufferEntity = GetSingletonEntity<TeamReformCommand>();

        var entityManager = EntityManager;

        Entities
            .WithAll<CaptainTag>()
            .ForEach((ref Home home, in MyTeam team) =>
            {
                var teamInfo = entityManager.GetComponentData<TeamInfo>(team.Value);
                var fetcherHome = entityManager.GetComponentData<Home>(teamInfo.Fetcher);

                var spot = BucketBrigadeUtility.FindClosestFireSpot(heatmapData, heatmap, fetcherHome.Value);
                
                if (!BucketBrigadeUtility.IsVeryClose(spot, home.Value))
                {
                    home.Value = spot;
                    ecb.AppendToBuffer(teamReformBufferEntity, new TeamReformCommand(team.Value));
                }
            })
            .Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
