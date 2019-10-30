using System.Collections.Generic;

namespace Unity.Coding.Editor.ApiScraping
{
    public interface IApiScraperProcess
    {
        void Start(IEnumerable<(string assemblyFullPath, string outputPath)> scrapeList, IEnumerable<string> referenceDirectories, ScrapeMode scrapeMode);
    }
}
