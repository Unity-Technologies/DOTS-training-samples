using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CompilationPipeline = UnityEditor.Compilation.CompilationPipeline;

namespace Unity.Build
{
    public class BuildTypeCache
    {
        readonly HashSet<Assembly> m_AssembliesCache = new HashSet<Assembly>();
        bool m_IsDirty = true;

        string m_PlatformName;
        List<string> m_ExcludedAssemblies = new List<string>();

        public string PlatformName
        {
            get => m_PlatformName;
            set
            {
                if (m_PlatformName != value)
                {
                    m_PlatformName = value;
                    m_IsDirty = true;
                }
            }
        }

        public List<string> ExcludedAssemblies
        {
            get => m_ExcludedAssemblies;
            set
            {
                if (m_ExcludedAssemblies != value)
                {
                    m_ExcludedAssemblies = value;
                    m_IsDirty = true;
                }
            }
        }

        public bool HasType<T>()
        {
            return HasType(typeof(T));
        }

        public bool HasType(Type type)
        {
            RebuildCacheIfDirty();
            return m_AssembliesCache.Contains(type.Assembly);
        }

        public bool HasAssembly(Assembly assembly)
        {
            RebuildCacheIfDirty();
            return m_AssembliesCache.Contains(assembly);
        }

        public bool HasAssembly(string assemblyName)
        {
            RebuildCacheIfDirty();
            return m_AssembliesCache.Any(assembly => assembly.GetName().Name == assemblyName);
        }

        public void SetDirty()
        {
            m_IsDirty = true;
        }

        void RebuildCacheIfDirty()
        {
            if (!m_IsDirty)
            {
                return;
            }

            m_AssembliesCache.Clear();

            var domainAssembliesMap = GetDomainAssembliesByName();
            var includedAssemblyDefinitions = FindMatchingAssemblyDefinitions();
            foreach (var assemblyDefinition in includedAssemblyDefinitions)
            {
                if (domainAssembliesMap.TryGetValue(assemblyDefinition.name, out var assembly))
                {
                    m_AssembliesCache.Add(assembly);
                }
            }

            m_IsDirty = false;
        }

        static Dictionary<string, Assembly> GetDomainAssembliesByName()
        {
            var assemblies = new Dictionary<string, Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var name = assembly.GetName().Name;
                if (!assemblies.ContainsKey(name))
                {
                    assemblies.Add(name, assembly);
                }
            }
            return assemblies;
        }

        IEnumerable<AssemblyDefinition> FindMatchingAssemblyDefinitions()
        {
            var assemblyNames = new HashSet<string>();
            var assemblyDefinitions = new Stack<AssemblyDefinition>();

            var projectAssemblyDefinitions = UnityEditor.AssetDatabase.FindAssets("t: asmdef", new string[] { "Assets" })
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(AssemblyDefinition.Deserialize)
                .Where(asmdef => asmdef.MatchPlatform(m_PlatformName));

            var autoReferencedAssemblyDefinitions = UnityEditor.AssetDatabase.FindAssets("t: asmdef")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(AssemblyDefinition.Deserialize)
                .Where(asmdef => asmdef.autoReferenced && asmdef.MatchPlatform(m_PlatformName));

            foreach (var assemblyDefinition in projectAssemblyDefinitions.Union(autoReferencedAssemblyDefinitions))
            {
                assemblyNames.Add(assemblyDefinition.name);
                assemblyDefinitions.Push(assemblyDefinition);
            }

            // Walk the assembly tree from main assemblies
            var result = new HashSet<AssemblyDefinition>();
            while (assemblyDefinitions.Count > 0)
            {
                var assemblyDefinition = assemblyDefinitions.Pop();
                if (assemblyDefinition.references == null)
                {
                    continue;
                }

                if (m_ExcludedAssemblies?.Contains(assemblyDefinition.name) ?? false)
                {
                    continue;
                }

                if (!result.Add(assemblyDefinition))
                {
                    continue;
                }

                foreach (var assemblyReference in assemblyDefinition.references)
                {
                    if (assemblyNames.Contains(assemblyReference))
                    {
                        continue;
                    }

                    var assemblyDefinitionPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(assemblyReference);
                    if (string.IsNullOrEmpty(assemblyDefinitionPath))
                    {
                        continue;
                    }

                    var referencedAssemblyDefinition = AssemblyDefinition.Deserialize(assemblyDefinitionPath);
                    if (assemblyNames.Add(referencedAssemblyDefinition.name) && referencedAssemblyDefinition.MatchPlatform(m_PlatformName))
                    {
                        assemblyDefinitions.Push(referencedAssemblyDefinition);
                    }
                }
            }

            return result;
        }

        class AssemblyDefinition
        {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
            public string name;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public string[] optionalUnityReferences;
            public bool autoReferenced;
#pragma warning restore CS0649

            public static AssemblyDefinition Deserialize(string assemblyDefinitionPath)
            {
                var assemblyDefinition = new AssemblyDefinition { autoReferenced = true }; // autoReferenced is true by default even when it's not defined
                UnityEngine.JsonUtility.FromJsonOverwrite(File.ReadAllText(assemblyDefinitionPath), assemblyDefinition);

                if (assemblyDefinition == null)
                {
                    throw new Exception($"File '{assemblyDefinitionPath}' does not contain valid asmdef data.");
                }

                if (string.IsNullOrEmpty(assemblyDefinition.name))
                {
                    throw new Exception($"Required property '{nameof(name)}' not set.");
                }

                if (assemblyDefinition.excludePlatforms?.Length > 0 && assemblyDefinition.includePlatforms?.Length > 0)
                {
                    throw new Exception($"Both '{nameof(excludePlatforms)}' and '{nameof(includePlatforms)}' are set.");
                }

                return assemblyDefinition;
            }

            public bool MatchPlatform(string platformName)
            {
                // Ignore test assemblies
                if (optionalUnityReferences != null && optionalUnityReferences.Any(r => r == "TestAssemblies"))
                {
                    return false;
                }

                var emptyIncludes = includePlatforms == null || includePlatforms.Length == 0;
                var emptyExcludes = excludePlatforms == null || excludePlatforms.Length == 0;

                // If no included or excluded platforms, means its available for any platforms
                if (emptyIncludes && emptyExcludes)
                {
                    return true;
                }

                // Is there a valid platform name?
                if (string.IsNullOrEmpty(platformName))
                {
                    return false;
                }

                // Not listed in included platforms
                if (!emptyIncludes && !includePlatforms.Contains(platformName))
                {
                    return false;
                }

                // Not listed in excluded platforms
                return emptyExcludes || !excludePlatforms.Contains(platformName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                AssemblyDefinition other = (AssemblyDefinition)obj;
                return name == other.name;
            }

            public override int GetHashCode() => name.GetHashCode();

            public override string ToString() => name;
        }
    }
}
