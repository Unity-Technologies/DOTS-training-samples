using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class GenerateBundleMaps : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext]
        IBundleWriteData m_WriteData;
#pragma warning restore 649

        public ReturnCode Run()
        {
            Dictionary<string, WriteCommand> fileToCommand = m_WriteData.WriteOperations.ToDictionary(x => x.Command.internalName, x => x.Command);
            Dictionary<string, HashSet<string>> filesMapped = new Dictionary<string, HashSet<string>>();
            foreach (var assetFilesPair in m_WriteData.AssetToFiles)
            {
                // Generate BuildReferenceMap map
                AddReferencesForFiles(assetFilesPair.Value, filesMapped, fileToCommand);

                // Generate BuildUsageTagSet map
                AddUsageSetForFiles(assetFilesPair.Key, assetFilesPair.Value);
            }

            return ReturnCode.Success;
        }

        void AddReferencesForFiles(IList<string> files, Dictionary<string, HashSet<string>> filesMapped, Dictionary<string, WriteCommand> fileToCommand)
        {
            HashSet<string> visited;
            filesMapped.GetOrAdd(files[0], out visited);

            BuildReferenceMap referenceMap;
            if (!m_WriteData.FileToReferenceMap.TryGetValue(files[0], out referenceMap))
            {
                referenceMap = new BuildReferenceMap();
                m_WriteData.FileToReferenceMap.Add(files[0], referenceMap);
            }

            foreach (var file in files)
            {
                if (!visited.Add(file))
                    continue;

                var command = fileToCommand[file];
                referenceMap.AddMappings(file, command.serializeObjects.ToArray());
            }
        }

        void AddUsageSetForFiles(GUID asset, IList<string> files)
        {
            BuildUsageTagSet assetUsage;
            if (!m_DependencyData.AssetUsage.TryGetValue(asset, out assetUsage))
            {
                if (!m_DependencyData.SceneUsage.TryGetValue(asset, out assetUsage))
                    return;
            }

            foreach (var file in files)
            {
                BuildUsageTagSet fileUsage;
                if (!m_WriteData.FileToUsageSet.TryGetValue(file, out fileUsage))
                {
                    fileUsage = new BuildUsageTagSet();
                    m_WriteData.FileToUsageSet.Add(file, fileUsage);
                }
                fileUsage.UnionWith(assetUsage);
            }
        }
    }
}
