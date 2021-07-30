using Unity.Profiling;

public class GameStats
{
    public static readonly ProfilerCategory BeeProfilerCategory = ProfilerCategory.Scripts;

    public static readonly ProfilerCounter<int> BeeCount =
        new ProfilerCounter<int>(BeeProfilerCategory, "Bee Count", ProfilerMarkerDataUnit.Count);
    
    public static readonly ProfilerCounter<int> ResourceCount =
        new ProfilerCounter<int>(BeeProfilerCategory, "Resource Count", ProfilerMarkerDataUnit.Count);
    
    public static readonly ProfilerCounter<int> BloodCount =
        new ProfilerCounter<int>(BeeProfilerCategory, "Blood Count", ProfilerMarkerDataUnit.Count);
}