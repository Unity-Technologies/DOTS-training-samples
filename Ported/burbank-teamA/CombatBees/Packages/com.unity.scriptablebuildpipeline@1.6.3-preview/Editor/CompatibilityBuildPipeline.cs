using System.IO;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEngine;
using UnityEngine.Build.Pipeline;

namespace UnityEditor.Build.Pipeline
{
#if UNITY_2018_3_OR_NEWER
    using BuildCompression = UnityEngine.BuildCompression;
#else
    using BuildCompression = UnityEditor.Build.Content.BuildCompression;
#endif
    
    /// <summary>
    /// Static class implementing API wrappers to match BuildPipeline APIs but use the Scriptable Build Pipeline.
    /// <seealso cref="BuildPipeline.BuildAssetBundles"/>
    /// </summary>
    public static class CompatibilityBuildPipeline
    {
        /// <summary>
        /// Wrapper API to match BuildPipeline API but use the Scriptable Build Pipeline to build Asset Bundles.
        /// <seealso cref="BuildPipeline.BuildAssetBundles(string, BuildAssetBundleOptions, BuildTarget)"/>
        /// </summary>
        /// <remarks>
        /// Not all BuildAssetBundleOptions are supported in the Scriptable Build Pipeline initial release.
        /// Supported options are: ChunkBasedCompression, UncompressedAssetBundle, DisableWriteTypeTree, and DisableWriteTypeTree.
        /// In addition, existing BuildPipeline callbacks are not yet supported. Est: 2018.3 release for support.
        /// </remarks>
        /// <param name="outputPath">Output path for the AssetBundles.</param>
        /// <param name="assetBundleOptions">AssetBundle building options.</param>
        /// <param name="targetPlatform">Chosen target build platform.</param>
        /// <returns>null - Generating and returning an AssetBundleManifest is not yet supported by the Scriptable Build Pipeline. Est: 2018.3 support.</returns>
        public static CompatibilityAssetBundleManifest BuildAssetBundles(string outputPath, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            var buildInput = ContentBuildInterface.GenerateAssetBundleBuilds();
            return BuildAssetBundles_Internal(outputPath, new BundleBuildContent(buildInput), assetBundleOptions, targetPlatform);
        }

        /// <summary>
        /// Wrapper API to match BuildPipeline API but use the Scriptable Build Pipeline to build Asset Bundles.
        /// <seealso cref="BuildPipeline.BuildAssetBundles(string, AssetBundleBuild[], BuildAssetBundleOptions, BuildTarget)"/>
        /// </summary>
        /// <remarks>
        /// Not all BuildAssetBundleOptions are supported in the Scriptable Build Pipeline initial release.
        /// Supported options are: ChunkBasedCompression, UncompressedAssetBundle, DisableWriteTypeTree, and DisableWriteTypeTree.
        /// In addition, existing BuildPipeline callbacks are not yet supported. Est: 2018.3 release for support.
        /// </remarks>
        /// <param name="outputPath">Output path for the AssetBundles.</param>
        /// <param name="builds">AssetBundle building map.</param>
        /// <param name="assetBundleOptions">AssetBundle building options.</param>
        /// <param name="targetPlatform">Chosen target build platform.</param>
        /// <returns>null - Generating and returning an AssetBundleManifest is not yet supported by the Scriptable Build Pipeline. Est: 2018.3 release for support.</returns>
        public static CompatibilityAssetBundleManifest BuildAssetBundles(string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
        {
            return BuildAssetBundles_Internal(outputPath, new BundleBuildContent(builds), assetBundleOptions, targetPlatform);
        }

        internal static CompatibilityAssetBundleManifest BuildAssetBundles_Internal(string outputPath, IBundleBuildContent content, BuildAssetBundleOptions options, BuildTarget targetPlatform)
        {
            var group = BuildPipeline.GetBuildTargetGroup(targetPlatform);
            var parameters = new BundleBuildParameters(targetPlatform, group, outputPath);
            if ((options & BuildAssetBundleOptions.ForceRebuildAssetBundle) != 0)
                parameters.UseCache = false;
			
			if ((options & BuildAssetBundleOptions.AppendHashToAssetBundleName) != 0)
				parameters.AppendHash = true;

#if UNITY_2018_3_OR_NEWER
            if ((options & BuildAssetBundleOptions.ChunkBasedCompression) != 0)
                parameters.BundleCompression = BuildCompression.LZ4;
            else if ((options & BuildAssetBundleOptions.UncompressedAssetBundle) != 0)
                parameters.BundleCompression = BuildCompression.Uncompressed;
            else
                parameters.BundleCompression = BuildCompression.LZMA;
#else
            if ((options & BuildAssetBundleOptions.ChunkBasedCompression) != 0)
                parameters.BundleCompression = BuildCompression.DefaultLZ4;
            else if ((options & BuildAssetBundleOptions.UncompressedAssetBundle) != 0)
                parameters.BundleCompression = BuildCompression.DefaultUncompressed;
            else
                parameters.BundleCompression = BuildCompression.DefaultLZMA;
#endif

            if ((options & BuildAssetBundleOptions.DisableWriteTypeTree) != 0)
                parameters.ContentBuildFlags |= ContentBuildFlags.DisableWriteTypeTree;

            IBundleBuildResults results;
            ReturnCode exitCode = ContentPipeline.BuildAssetBundles(parameters, content, out results);
            if (exitCode < ReturnCode.Success)
                return null;

            var manifest = ScriptableObject.CreateInstance<CompatibilityAssetBundleManifest>();
            manifest.SetResults(results.BundleInfos);
            File.WriteAllText(parameters.GetOutputFilePathForIdentifier(Path.GetFileName(outputPath) + ".manifest"), manifest.ToString());
            return manifest;
        }
    }
}