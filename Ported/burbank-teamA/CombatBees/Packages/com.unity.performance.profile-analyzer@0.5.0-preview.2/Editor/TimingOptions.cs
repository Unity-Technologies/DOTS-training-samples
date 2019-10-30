namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class TimingOptions
    {
        public enum TimingOption
        {
            Time,
            Self,
        };

        public static string[] TimingOptionNames = 
        {
            "Total",
            "Self",
        };

        public static string Tooltip = "Marker timings :\n\nTotal : \tIncluding children\nSelf : \tExcluding children";
    }
}
