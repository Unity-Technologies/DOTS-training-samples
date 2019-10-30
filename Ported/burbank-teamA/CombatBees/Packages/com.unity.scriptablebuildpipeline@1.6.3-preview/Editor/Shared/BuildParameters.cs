using System;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;
using UnityEngine;

namespace UnityEditor.Build.Pipeline
{
#if UNITY_2018_3_OR_NEWER
    using BuildCompression = UnityEngine.BuildCompression;
#else
    using BuildCompression = UnityEditor.Build.Content.BuildCompression;
#endif
    
    /// <summary>
    /// Basic implementation of IBuildParameters. Stores the set of parameters passed into the Scriptable Build Pipeline.
    /// <seealso cref="IBuildParameters"/>
    /// </summary>
    [Serializable]
    public class BuildParameters : IBuildParameters
    {
        /// <inheritdoc />
        public BuildTarget Target { get; set; }
        /// <inheritdoc />
        public BuildTargetGroup Group { get; set; }

        /// <inheritdoc />
        public ContentBuildFlags ContentBuildFlags { get; set; }

        /// <inheritdoc />
        public TypeDB ScriptInfo { get; set; }
        /// <inheritdoc />
        public ScriptCompilationOptions ScriptOptions { get; set; }

        /// <summary>
        /// Default compression option to use for all built content files
        /// </summary>
        public BuildCompression BundleCompression { get; set; }

        /// <summary>
        /// Final output location where built content will be written.
        /// </summary>
        public string OutputFolder { get; set; }

        string m_TempOutputFolder;
        /// <inheritdoc />
        public string TempOutputFolder
        {
            get { return m_TempOutputFolder; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException("Argument cannot be null or empty.", "value");
                m_TempOutputFolder = value;
            }
        }

        /// <inheritdoc />
        public bool UseCache { get; set; }
        /// <inheritdoc />
        public string CacheServerHost { get; set; }
        /// <inheritdoc />
        public int CacheServerPort { get; set; }

        internal BuildParameters() { }

        /// <summary>
        /// Default constructor, requires the target, group and output parameters at minimum for a successful build.
        /// </summary>
        /// <param name="target">The target for building content.</param>
        /// <param name="group">The group for building content.</param>
        /// <param name="outputFolder">The final output location for built content.</param>
        public BuildParameters(BuildTarget target, BuildTargetGroup group, string outputFolder)
        {
            if (string.IsNullOrEmpty(outputFolder))
                throw new ArgumentException("Argument cannot be null or empty.", "outputFolder");

            Target = target;
            Group = group;
            // TODO: Validate target & group

            ScriptInfo = null;
            ScriptOptions = ScriptCompilationOptions.None;
#if UNITY_2018_3_OR_NEWER
            BundleCompression = BuildCompression.LZMA;
#else
            BundleCompression = BuildCompression.DefaultLZMA;
#endif
            OutputFolder = outputFolder;
            TempOutputFolder = ContentPipeline.kTempBuildPath;
            UseCache = true;
            CacheServerPort = 8126;
        }

        /// <inheritdoc />
        public virtual BuildSettings GetContentBuildSettings()
        {
            return new BuildSettings
            {
                group = Group,
                target = Target,
                typeDB = ScriptInfo,
                buildFlags = ContentBuildFlags
            };
        }

        /// <inheritdoc />
        public virtual ScriptCompilationSettings GetScriptCompilationSettings()
        {
            return new ScriptCompilationSettings
            {
                group = Group,
                target = Target,
                options = ScriptOptions
            };
        }

        /// <inheritdoc />
        public virtual string GetOutputFilePathForIdentifier(string identifier)
        {
            return string.Format("{0}/{1}", OutputFolder, identifier);
        }

        /// <inheritdoc />
        public virtual BuildCompression GetCompressionForIdentifier(string identifier)
        {
            return BundleCompression;
        }
    }

    [Serializable]
    public class BundleBuildParameters : BuildParameters, IBundleBuildParameters
    {
        internal BundleBuildParameters() { }

        /// <inheritdoc />
        public BundleBuildParameters(BuildTarget target, BuildTargetGroup group, string outputFolder)
            : base(target, group, outputFolder) { }

        /// <inheritdoc />
        public bool AppendHash { get; set; }
    }
}