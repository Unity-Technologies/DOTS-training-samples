using System;
using System.Collections.Generic;

namespace Unity.Coding.Editor.ApiScraping
{
    public class ScraperEntry
    {
        public ScraperEntry(string assemlyFullPath, string outputPath, params string[] referenceDirectories)
        {
            AssemblyFullPath = assemlyFullPath;
            OutputPath = outputPath;
            ReferenceDirectories = referenceDirectories;
        }

        public string AssemblyFullPath;
        public string OutputPath;
        public IEnumerable<string> ReferenceDirectories;
    }
}
