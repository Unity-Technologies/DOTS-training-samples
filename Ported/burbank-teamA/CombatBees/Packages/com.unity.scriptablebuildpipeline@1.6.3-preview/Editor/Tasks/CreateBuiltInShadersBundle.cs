using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Tasks
{
    /// <summary>
    /// Optional build task that extracts Unity's built in shaders and assigns them to the specified bundle 
    /// </summary>
    public class CreateBuiltInShadersBundle : IBuildTask
    {
        static readonly GUID k_BuiltInGuid = new GUID("0000000000000000f000000000000000");
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.InOut, true)]
        IBundleExplictObjectLayout m_Layout;
#pragma warning restore 649

        public string ShaderBundleName { get; set; }

        public CreateBuiltInShadersBundle(string bundleName)
        {
            ShaderBundleName = bundleName;
        }

        public ReturnCode Run()
        {
            HashSet<ObjectIdentifier> buildInObjects = new HashSet<ObjectIdentifier>();
            foreach (AssetLoadInfo dependencyInfo in m_DependencyData.AssetInfo.Values)
                buildInObjects.UnionWith(dependencyInfo.referencedObjects.Where(x => x.guid == k_BuiltInGuid));

            foreach (SceneDependencyInfo dependencyInfo in m_DependencyData.SceneInfo.Values)
                buildInObjects.UnionWith(dependencyInfo.referencedObjects.Where(x => x.guid == k_BuiltInGuid));

            ObjectIdentifier[] usedSet = buildInObjects.ToArray();
            Type[] usedTypes = ContentBuildInterface.GetTypeForObjects(usedSet);

            if (m_Layout == null)
                m_Layout = new BundleExplictObjectLayout();

            Type shader = typeof(Shader);
            for (int i = 0; i < usedTypes.Length; i++)
            {
                if (usedTypes[i] != shader)
                    continue;

                m_Layout.ExplicitObjectLocation.Add(usedSet[i], ShaderBundleName);
            }

            if (m_Layout.ExplicitObjectLocation.Count == 0)
                m_Layout = null;

            return ReturnCode.Success;
        }
    }
}
