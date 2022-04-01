using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class FireSpotLookupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var frameOffset = BucketBrigadeUtility.GetCurrentFrame() % BucketBrigadeUtility.FramesPerFireCheck;
        
        var heatmapData = GetSingleton<HeatMapData>();
        var heatmap = BucketBrigadeUtility.GetHeatmapBuffer(this);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var teamReformBufferEntity = GetSingletonEntity<TeamReformCommand>();

        var entityManager = EntityManager;

        Entities
            .WithAll<CaptainTag>()
            .ForEach((ref Home home, in MyTeam team, in EvalOffsetFrame offset) =>
            {
                if (offset.Value == frameOffset)
                {
                    var teamInfo = entityManager.GetComponentData<TeamInfo>(team.Value);
                    var fetcherHome = entityManager.GetComponentData<Home>(teamInfo.Fetcher);

                    var spot = BucketBrigadeUtility.FindClosestFireSpot(heatmapData, heatmap, fetcherHome.Value);

                    if (!BucketBrigadeUtility.IsVeryClose(spot, home.Value))
                    {
                        home.Value = spot;
                        ecb.AppendToBuffer(teamReformBufferEntity, new TeamReformCommand(team.Value));
                    }
                }
            })
            .Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
