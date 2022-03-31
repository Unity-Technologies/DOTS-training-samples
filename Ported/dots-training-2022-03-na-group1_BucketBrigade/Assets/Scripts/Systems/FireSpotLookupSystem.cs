using Unity.Entities;

public partial class FireSpotLookupSystem : SystemBase
{
    /// <summary>
    ///     The delay between each lookup
    /// </summary>
    const double k_Delay = 1;

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

        Entities
            .WithAll<FetcherTag>()
            .ForEach((in Home home) =>
            {
                var spot = BucketBrigadeUtility.FindClosestFireSpot(heatmapData, heatmap, home.Value);

                // TODO: Trying to set the home position of the captain of the group, and reform team.
                if (BucketBrigadeUtility.IsVeryClose(spot, default))
                {
                    //
                }
            })
            .Run();
    }
}
