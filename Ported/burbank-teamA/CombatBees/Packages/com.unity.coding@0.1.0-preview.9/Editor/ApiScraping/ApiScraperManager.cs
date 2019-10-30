using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Unity.Coding.Editor.ApiScraping
{
    class ApiScraperManager
    {
        static Profiling.ProfilerMarker s_UpdateTimestamp = new Profiling.ProfilerMarker("UpdateTimestamp");
        static Profiling.ProfilerMarker s_ProcessStart = new Profiling.ProfilerMarker("ApiScraperProcess.Start");
        const string k_TimestampsFolder = "Library/com.unity.coding/ApiScraper/Timestamps/";

        readonly IApiScraperProcess m_Process;

        public ScrapeMode scrapeMode { get; set; }

        public ApiScraperManager(IApiScraperProcess process, ScrapeMode scrapeMode = ScrapeMode.Asynchronous)
        {
            m_Process = process;
            this.scrapeMode = scrapeMode;
        }

        public void RunScraper(IList<ScraperEntry> entries)
        {
            if (entries.Count == 0)
                return;

            var scrapeList = entries.Select(e => (e.AssemblyFullPath, e.OutputPath));
            var referencedFolders = entries.First().ReferenceDirectories;

            using (s_ProcessStart.Auto())
                m_Process.Start(scrapeList, referencedFolders, scrapeMode);

            using (s_UpdateTimestamp.Auto())
            {
                foreach (var entry in entries)
                {
                    var timestampFilePath = TimestampFilePathFor(entry.AssemblyFullPath);
                    UpdateTimestamp(entry.AssemblyFullPath, timestampFilePath);
                }
            }
        }

        internal static string TimestampFilePathFor(string assemblyFullPath)
        {
            var assemblyFileName = Path.GetFileName(assemblyFullPath);
            return Path.Combine(k_TimestampsFolder, assemblyFileName + ".timestamp");
        }

        void UpdateTimestamp(string assemblyFullPath, string timestampFilePath)
        {
            if (!Directory.Exists(k_TimestampsFolder))
                Directory.CreateDirectory(k_TimestampsFolder);

            File.WriteAllText(timestampFilePath, File.GetLastWriteTime(assemblyFullPath).Ticks.ToString());
        }

        public bool TimestampMatches(string assemblyPath)
        {
            var timestampFilePath = TimestampFilePathFor(assemblyPath);
            if (!File.Exists(timestampFilePath))
                return false;

            return File.ReadAllText(timestampFilePath) == File.GetLastWriteTime(assemblyPath).Ticks.ToString();
        }

        public static void DeleteAllTimestamps()
        {
            if (Directory.Exists(k_TimestampsFolder))
                Directory.Delete(k_TimestampsFolder, true);
        }
    }

    public enum ScrapeMode
    {
        Asynchronous,
        EnsureSuccess,
        VerifyNoWrite
    }
}
