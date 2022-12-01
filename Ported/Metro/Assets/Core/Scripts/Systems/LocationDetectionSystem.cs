using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct LocationDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        LocationJob locationJob = new LocationJob();
        locationJob.config = config;
        locationJob.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct LocationJob : IJobEntity
    {
        [ReadOnly] public Config config;
        public void Execute(in TransformAspect transform, ref LocationInfo locationInfo)
        {
            int stationSpacing = (int)(Globals.RailSize / config.NumberOfStations);
            int column = locationInfo.CurrentStation = (int)((Globals.RailSize*0.5f+transform.LocalPosition.z) / stationSpacing);
            int row = (int)((transform.LocalPosition.x) / Globals.PlatformSpacing);
            locationInfo.CurrentPlatform = (row * config.NumberOfStations) + column;
        }
    }
}