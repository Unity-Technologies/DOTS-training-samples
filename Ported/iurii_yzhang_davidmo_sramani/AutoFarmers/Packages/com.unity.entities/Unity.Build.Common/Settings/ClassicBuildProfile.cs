using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PropertyAttribute = Unity.Properties.PropertyAttribute;

namespace Unity.Build.Common
{
    public sealed class ClassicBuildProfile : IBuildProfileComponent
    {
        BuildTarget m_Target;
        List<string> m_ExcludedAssemblies;

        /// <summary>
        /// Retrieve <see cref="BuildTypeCache"/> for this build profile.
        /// </summary>
        public BuildTypeCache TypeCache { get; } = new BuildTypeCache();

        /// <summary>
        /// Gets or sets which <see cref="BuildTarget"/> this profile is going to use for the build.
        /// Used for building classic Unity standalone players.
        /// </summary>
        [Property]
        public BuildTarget Target
        {
            get => m_Target;
            set
            {
                m_Target = value;
                TypeCache.PlatformName = m_Target.ToString();
            }
        }

        /// <summary>
        /// Gets or sets which <see cref="Configuration"/> this profile is going to use for the build.
        /// </summary>
        [Property]
        public BuildConfiguration Configuration { get; set; } = BuildConfiguration.Develop;

        [Property]
        public BuildPipeline Pipeline { get; set; }

        /// <summary>
        /// List of assemblies that should be explicitly excluded for the build.
        /// </summary>
        [Property, HideInInspector]
        public List<string> ExcludedAssemblies
        {
            get => m_ExcludedAssemblies;
            set
            {
                m_ExcludedAssemblies = value;
                TypeCache.ExcludedAssemblies = value;
            }
        }

        public ClassicBuildProfile()
        {
            Target = BuildTarget.NoTarget;
            ExcludedAssemblies = new List<string>();
        }

        #region IBuildProfileComponent

        public Platforms.BuildTarget GetBuildTarget()
        {
            if (m_Target == BuildTarget.StandaloneWindows)
                m_Target = BuildTarget.StandaloneWindows64;

#pragma warning disable CS0618
            if (m_Target == BuildTarget.StandaloneOSXIntel || m_Target == BuildTarget.StandaloneOSXIntel64)
                m_Target = BuildTarget.StandaloneOSX;

            if (m_Target == BuildTarget.StandaloneLinux || m_Target == BuildTarget.StandaloneLinuxUniversal)
                m_Target = BuildTarget.StandaloneLinux64;
#pragma warning restore CS0618

            var buildTarget = Platforms.BuildTarget.GetBuildTargetFromUnityPlatformName(m_Target.ToString());
            if (buildTarget == null)
            {
                Debug.LogFormat($"Unable to find {nameof(Platforms.BuildTarget)} from {nameof(BuildTarget)} value {m_Target.ToString()}.");
            }
            return buildTarget;
        }

        public BuildPipeline GetBuildPipeline() => Pipeline;

        #endregion

        internal string GetExecutableExtension()
        {
#pragma warning disable CS0618
            switch (m_Target)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return ".app";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.NoTarget:
                case BuildTarget.StandaloneLinux64:
                    return string.Empty;
                default:
                    throw new ArgumentException($"Invalid or unhandled enum {m_Target.ToString()} (index {(int)m_Target})");
            }
#pragma warning restore CS0618
        }
    }
}
