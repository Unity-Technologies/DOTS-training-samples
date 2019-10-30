# Upgrade Guide

To build your AssetBundles with the SBP package, use the `CompatibilityBuildPipeline.BuildAssetBundles` method wherever you used the `BuildPipeline.BuildAssetBundle` method.

**Note:** Not all of the features that were supported previously are supported in SBP.

The following tables list the features of the `CompatibilityBuildPipeline.BuildAssetBundles` method in comparison to the `BuildPipeline.BuildAssetBundle` method.

| Feature| Support | Notes |
|:---|:---|:---|
| AssetBundles| Supported | SBP builds AssetBundles built nearly identically to the previous build pipeline. You load them in a similar manner to how you currently load AssetBundles.  |
| Incremental Building | Supported | SBP implements this feature using the new `BuildCache` class. |
| Asset loading path| Behavior changed | AssetBundles built with BuildPipeline today support loading an Asset by full path: *Assets/ExampleFolder/Asset.prefab*, file name: *Asset*, or file name with extension: *Asset.prefab*. However AssetBundles built with SBP by default only support loading an Asset by full path: *Assets/ExampleFolder/Asset.prefab*. This is to avoid loading collision that can occur if two Assets in the same AssetBundle have the different full paths, but the same file name. To change this behavior, the loading path can be set using `IBundleBuildContent.Addresses` with the `ContentPipeline.BuildAssetBundles` API or use the [AssetBundleBuild.addressableNames](https://docs.unity3d.com/ScriptReference/AssetBundleBuild-addressableNames.html) field. See [Usage Examples](UsageExamples.md). |
| AssetBundle Manifest | Behavior changed | SBP implements replacement functionality using the new class name `CompatibilityAssetBundleManifest`. This has an identical API to the existing `AssetBundleManifest` class, and has an additional method to get the CRC value for a bundle which did not exist before. |
| AssetBundle Variants| Not supported | There is currently no replacement functionality for AssetBundle Variants. |

BuildAssetBundleOptions Enum:

| Value| Support | Notes |
|:---|:---|:---|
| UncompressedAssetBundle| Supported | Identical to using `BuildCompression.DefaultUncompressed`. |
| ChunkBasedCompression | Supported | Identical to using `BuildCompression.DefaultLZ4`. **Note:** This has always been LZ4HC in the Editor, and LZ4 if it was recompressed at Runtime. |
| DisableWriteTypeTree | Supported | Identical to using `ContentBuildFlags.DisableWriteTypeTree`. |
| DeterministicAssetBundle | Supported | This is enabled by default, and it can’t be disabled. SBP builds deterministically. |
| ForceRebuildAssetBundle | Supported | Identical to using `IBuildParameters.UseCache = false;`. |
| AppendHashToAssetBundleName | Supported | Identical to using `IBundleBuildParameters.AppendHash = true;`.  |
| DisableLoadAssetByFileName | Always enabled | This is enabled by default, and can’t be disabled. SBP is strict about the rule: "what you pass in is exactly what you get out". If you pass in *My/Example1/Example2/Asset.asset* as the file name to use to load the Asset, you must use that identifier exactly, including the correct upper and lower case, and all punctuation. |
| DisableLoadAssetByFileNameWithExtension | Always enabled | See above details on DisableLoadAssetByFileName. |
| IgnoreTypeTreeChanges | Not supported | The incremental build system used this value to prevent rebuilding AssetBundles when an Asset's serialization layout changed, but the data for the Asset itself did not change. SBP currently rebuilds if there are any changes. |
| StrictMode | Not supported | The SBP is stricter about properly building AssetBundles and knowing when builds fail.  |
| DryRunBuild | Not supported | SBP works fundamentally differently. It is faster to do a full build to determine if anything has changed. |