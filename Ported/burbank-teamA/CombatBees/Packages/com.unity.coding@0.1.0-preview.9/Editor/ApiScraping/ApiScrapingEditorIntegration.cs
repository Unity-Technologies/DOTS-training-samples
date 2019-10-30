using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using System.Linq;
using Unity.Coding.Utils;
using System.Collections.Generic;

namespace Unity.Coding.Editor.ApiScraping
{
    class ApiScrapingSingleton : ScriptableSingleton<ApiScrapingSingleton>
    {
        public bool hasDoneFullScan;

        //for tests. If someone overrides scrape mode during this process, we will not call Exit in batch mode.
        public bool isScrapeModeOverridden;
    }

    [InitializeOnLoad]
    static class ApiScrapingEditorIntegration
    {
        static IList<string> s_AssemblyPathsToScrape;
        static ApiScrapingEditorIntegration()
        {
            s_Process = new ApiScraperProcess();
            s_Scraper = new ApiScraperManager(s_Process);

            if (Environment.GetCommandLineArgs().Contains("runTests") || Environment.GetCommandLineArgs().Contains("runEditorTests"))
            {
                // let's disable the scraper for test runs
                return;
            }

            s_AssemblyPathsToScrape = new List<string>();
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
            scrapeOnAssemblyCompilationFinished = true;
            if (!ApiScrapingSingleton.instance.hasDoneFullScan)
            {
                ApiScrapingSingleton.instance.hasDoneFullScan = true;
                ScanAndScrape();
            }
        }

        static void OnCompilationFinished(object obj)
        {
            // Don't scrape apis compiling players
            if(BuildPipeline.isBuildingPlayer)
                return;
            ScrapeIfConfigured(s_AssemblyPathsToScrape);
            s_AssemblyPathsToScrape.Clear();
        }

        internal static bool ValidateAll(List<string> failedFileList)
        {
            if(s_Process == null)
                s_Process = new ApiScraperProcess();
            if(s_Scraper == null)
                s_Scraper = new ApiScraperManager(s_Process);

            var failed = false;
            var callback = new Action<string>(delegate(string filePath)
            {
                failed = true;
                failedFileList?.Add(filePath);
            });
            var oldScrapeMode = s_Scraper.scrapeMode;
            try
            {
                ApiScraperManager.DeleteAllTimestamps();
                s_Scraper.scrapeMode = ScrapeMode.VerifyNoWrite;
                s_Process.verifyNoWriteFailed += callback;
                ScanAndScrape();
                s_AssemblyPathsToScrape.Clear();
            }
            finally
            {
                s_Scraper.scrapeMode = oldScrapeMode;
                s_Process.verifyNoWriteFailed -= callback;
            }
            return !failed;
        }

        static Profiling.ProfilerMarker s_IsChildOfProjectMarker = new Profiling.ProfilerMarker("IsChildOfProjectFolderOrLocalPackage");
        static Profiling.ProfilerMarker s_TimestampMatchesMarker = new Profiling.ProfilerMarker("TimestampMatches");
        static Profiling.ProfilerMarker s_GetAssemblyDefinitionFilePathMarker = new Profiling.ProfilerMarker("GetAssemblyDefinitionFilePath");
        static Profiling.ProfilerMarker s_ScanAndScrapeMarker = new Profiling.ProfilerMarker("ScanAndScrape");
        static Profiling.ProfilerMarker s_EditorConfigParsingMarker = new Profiling.ProfilerMarker("EditorConfigParsing");
        static Profiling.ProfilerMarker s_RunScraperMarker = new Profiling.ProfilerMarker("RunScraper");
        static Profiling.ProfilerMarker s_GetReferenceDirectoriesMarker = new Profiling.ProfilerMarker("GetReferenceDirectories");
        static Profiling.ProfilerMarker s_DirectoryGetFiles = new Profiling.ProfilerMarker("DirectoryGetFiles");

        static string[] s_ReferenceDirectories;
        internal static void ScanAndScrape(bool log = false)
        {
            IEnumerable<string> dlls;
            using (s_DirectoryGetFiles.Auto())
            {
                dlls = Directory.GetFiles("Library/ScriptAssemblies", "*.dll");
            }
            var scrapedDlls = ScrapeIfConfigured(dlls);
            if (log)
            {
                if (scrapedDlls.Count == 0)
                    Debug.Log("Updated 0 .api files.");
                else
                    Debug.Log($"Updated {scrapedDlls.Count} .api files:\n{string.Join("\n", scrapedDlls.Select(c => Path.GetFileName(c.AssemblyFullPath) ))}");
            }
        }

        static void OnAssemblyCompilationFinished(string path, CompilerMessage[] messages)
        {
            if (scrapeOnAssemblyCompilationFinished)
            {
                s_AssemblyPathsToScrape.Add(path);
            }
        }

        static ScraperEntry ScrapeIfConfigured(string path)
        {
            string asmdefPath;
            using (s_GetAssemblyDefinitionFilePathMarker.Auto())
            {
                asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(path);
            }

            if (asmdefPath != null && IsConfiguredToScrape(asmdefPath))
            {
                var fullOutputPath = Path.GetFullPath(path);
                var scrapeFilePath = Path.Combine(Path.GetDirectoryName(asmdefPath), Path.GetFileNameWithoutExtension(path) + ".api");
                using (s_TimestampMatchesMarker.Auto())
                {
                    if (s_Scraper.TimestampMatches(fullOutputPath) && File.Exists(scrapeFilePath))
                        return null;
                }

                using (s_GetReferenceDirectoriesMarker.Auto())
                {
                    // CompilationPipeline.GetAssemblies() is very slow, so instead we use the paths returned by
                    // CompilationPipeline.GetPrecompiledAssemblyPaths and the path of the assembly being scraped.
                    // There are plans to make a version of GetAssemblies that returns just one assembly and should be much faster.

                    // var actualAssembly = assembly ?? CompilationPipeline.GetAssemblies().First(a => a.outputPath == path);
                    // var referenceDirectories = actualAssembly.allReferences.Select(Path.GetDirectoryName).Distinct().ToArray();
                    var extensionsFolder = Path.Combine(EditorApplication.applicationContentsPath, "UnityExtensions/Unity");
                    if (s_ReferenceDirectories == null)
                    {
                        s_ReferenceDirectories = CompilationPipeline
                            .GetPrecompiledAssemblyPaths(CompilationPipeline.PrecompiledAssemblySources.All)
                            .Append(path)
                            .Select(Path.GetDirectoryName)
                            .Append(Path.Combine(extensionsFolder, "TestRunner"))
                            .Append(Path.Combine(extensionsFolder, "TestRunner/net35/unity-custom"))
                            .Append(Path.Combine(extensionsFolder, "GUISystem"))
                            .Append(Path.Combine(extensionsFolder, "GUISystem/Editor"))
                            .Append(Path.Combine(extensionsFolder, "UnityVR/Editor"))
                            .Distinct()
                            .Where(Directory.Exists)
                            .ToArray();
                    }
                }

                return new ScraperEntry(fullOutputPath, scrapeFilePath, s_ReferenceDirectories);
            }

            return null;
        }

        static IList<ScraperEntry> ScrapeIfConfigured(IEnumerable<string> paths)
        {
            var tbs = new List<ScraperEntry>();
            using (s_ScanAndScrapeMarker.Auto())
            {
                foreach (var path in paths)
                {
                    var entry = ScrapeIfConfigured(path);
                    if (entry == null)
                        continue;

                    tbs.Add(entry);
                }

                using (s_RunScraperMarker.Auto())
                {
                    s_Scraper.RunScraper(tbs);
                }
            }

            return tbs;
        }

        internal static bool IsConfiguredToScrape(string asmdefPath)
        {
            string fullAsmdefPath;
            using (s_IsChildOfProjectMarker.Auto())
            {
                //The original code used NPath and PackageInfo.FindForAssetPath(projectRelativePath.ToString(SlashMode.Forward));
                // It was changed to only use file system APIs for performance
                fullAsmdefPath = Path.GetFullPath(asmdefPath);
                var projectRootPath = Path.GetDirectoryName(Application.dataPath);
                if (!fullAsmdefPath.StartsWith(Path.Combine(projectRootPath, "Assets")) &&
                    !fullAsmdefPath.StartsWith(Path.Combine(projectRootPath, "Packages")))
                    return false;
            }

            using (s_EditorConfigParsingMarker.Auto())
            {
                // uncomment to use EditorConfig.Core instead of FastEditorConfigParser

                // var editorConfig = new EditorConfigParser().Parse(asmdefPath).FirstOrDefault();
                // if(editorConfig == null)
                //     return false;

                // if (!editorConfig.Properties.TryGetValue("scrape_api", out string scrapeApi))
                //     return false;

                var scrapeApi = FastEditorConfigParser.ParseForSingleValue(fullAsmdefPath.ToNPath(), "scrape_api");

                if (scrapeApi == null)
                    return false;

                if (!Boolean.TryParse(scrapeApi, out var shouldScrape))
                {
                    Debug.LogError($".editorconfig value '{scrapeApi}' is invalid for property scrape_api (valid values: true/false).");
                    return false;
                }
                
                return shouldScrape;
            }
        }

        static ApiScraperManager s_Scraper;
        static ApiScraperProcess s_Process;
        internal static bool scrapeOnAssemblyCompilationFinished { get; set; }

        internal static void ForceFullScrapeOnNextDomainReload()
        {
            ApiScrapingSingleton.instance.hasDoneFullScan = false;
        }

        internal static ScrapeMode scrapeMode
        {
            get => s_Scraper.scrapeMode;
            set
            {
                ApiScrapingSingleton.instance.isScrapeModeOverridden = true;
                s_Scraper.scrapeMode = value;
            }
        }
    }
}
