# Usage Examples

## Basic Example
This example assumes that your are already familiar with the basic usage of the two `BuildPipeline.BuildAssetBundles` methods and want to switch to using Scriptable Build Pipeline with as little effort  as possible. 

The following code example shows how AssetBundles are currently built:

```csharp
using System.IO;
using UnityEditor;

public static class BuildAssetBundlesExample
{
    public static bool BuildAssetBundles(string outputPath, bool forceRebuild, bool useChunkBasedCompression, BuildTarget buildTarget)
    {
        var options = BuildAssetBundleOptions.None;
        if (useChunkBasedCompression)
            options |= BuildAssetBundleOptions.ChunkBasedCompression;

        if (forceRebuild)
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

        Directory.CreateDirectory(outputPath);
        var manifest = BuildPipeline.BuildAssetBundles(outputPath, options, buildTarget);
        return manifest != null;
    }
}
```

To update the previous code example to use SBP instead, add a new `using UnityEditor.Build.Pipeline` and replace the call to `BuildPipeline.BuildAssetBundles` with `CompatibilityBuildPipeline.BuildAssetBundles` as shown below:

```csharp
using System.IO;
using UnityEditor;
// Added new using
using UnityEditor.Build.Pipeline;

public static class BuildAssetBundlesExample
{
    public static bool BuildAssetBundles(string outputPath, bool forceRebuild, bool useChunkBasedCompression, BuildTarget buildTarget)
    {
        var options = BuildAssetBundleOptions.None;
        if (useChunkBasedCompression)
            options |= BuildAssetBundleOptions.ChunkBasedCompression;

        if (forceRebuild)
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

        Directory.CreateDirectory(outputPath);
        // Replaced BuildPipeline.BuildAssetBundles with CompatibilityBuildPipeline.BuildAssetBundles here
        var manifest = CompatibilityBuildPipeline.BuildAssetBundles(outputPath, options, buildTarget);
        return manifest != null;
    }
}
```
**Notes:** Some changes in the SBP building and loading process do not match the BuildPipeline behavior. For more information on these changes, see [Upgrade Guide](UpgradeGuide.md).

## Cache Server Integration Example
This following example shows how to share build artifacts between team members or multiple machines to achieve faster build times.

Requirements:
1. A separate Cache Server instance dedicated to build artifacts. It cannot be shared as an Asset Cache Server as data collisions will occur.
2. The build code must use the `ContentPipeline.BuildAssetBundles` method. 
3. `BundleBuildParameters.UseCache` is set to true.
4. `BundleBuildParameters.CacheServerHost` and `BundleBuildParameters.CacheServerPort` are set to the cache server instance host or IP address and port respectively.

Example code:

```csharp
public static class BuildAssetBundlesExample
{
    public static bool BuildAssetBundles(string outputPath, bool useChunkBasedCompression, BuildTarget buildTarget, BuildTargetGroup buildGroup)
    {
        var buildContent = new BundleBuildContent(ContentBuildInterface.GenerateAssetBundleBuilds());
        var buildParams = new BundleBuildParameters(buildTarget, buildGroup, outputPath);
        // Set build parameters for connecting to the Cache Server
        buildParams.UseCache = true;
        buildParams.CacheServerHost = "buildcache.unitygames.com";
        buildParams.CacheServerPort = 8126;

        if (useChunkBasedCompression)
            buildParams.BundleCompression = BuildCompression.DefaultLZ4;

        IBundleBuildResults results;
        ReturnCode exitCode = ContentPipeline.BuildAssetBundles(buildParams, buildContent, out results);
        return exitCode == ReturnCode.Success;
    }
}
```

## Per-Bundle Compression Example
The following example shows how to build your AssetBundles using different compression levels for each AssetBundle.This is useful if you are planning on shipping part of your bundles as Lz4 or Uncompressed with Player and want to download the remainder as Lzma later. 

The simplest implementation is to create a custom build parameters class that inherits from `BundleBuildParameters` and override the `GetCompressionForIdentifier` method. Then construct and pass this into the `ContentPipeline.BuildAssetBundles` method.

```csharp
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Interfaces;

public static class BuildAssetBundlesExample
{
    // New parameters class inheriting from BundleBuildParameters
    class CustomBuildParameters : BundleBuildParameters
    {
        public Dictionary<string, BuildCompression> PerBundleCompression { get; set; }

        public CustomBuildParameters(BuildTarget target, BuildTargetGroup group, string outputFolder) : base(target, group, outputFolder)
        {
            PerBundleCompression = new Dictionary<string, BuildCompression>();
        }

        // Override the GetCompressionForIdentifier method with new logic
        public override BuildCompression GetCompressionForIdentifier(string identifier)
        {
            BuildCompression value;
            if (PerBundleCompression.TryGetValue(identifier, out value))
                return value;
            return BundleCompression;
        }
    }

    public static bool BuildAssetBundles(string outputPath, bool useChunkBasedCompression, BuildTarget buildTarget, BuildTargetGroup buildGroup)
    {
        var buildContent = new BundleBuildContent(ContentBuildInterface.GenerateAssetBundleBuilds());
        // Construct the new parameters class
        var buildParams = new CustomBuildParameters(buildTarget, buildGroup, outputPath);
        // Populate the bundle specific compression data
        buildParams.PerBundleCompression.Add("Bundle1", BuildCompression.DefaultUncompressed);
        buildParams.PerBundleCompression.Add("Bundle2", BuildCompression.DefaultLZMA);

        if (m_Settings.compressionType == CompressionType.None)
            buildParams.BundleCompression = BuildCompression.DefaultUncompressed;
        else if (m_Settings.compressionType == CompressionType.Lzma)
            buildParams.BundleCompression = BuildCompression.DefaultLZMA;
        else if (m_Settings.compressionType == CompressionType.Lz4 || m_Settings.compressionType == CompressionType.Lz4HC)
            buildParams.BundleCompression = BuildCompression.DefaultLZ4;

        IBundleBuildResults results;
        ReturnCode exitCode = ContentPipeline.BuildAssetBundles(buildParams, buildContent, out results);
        return exitCode == ReturnCode.Success;
    }
}
```

## Load By File Name Example
The following example shows how to use the `CompatibilityBuildPipeline` methods to load by a filename instead of the full path.

The example uses the `ContentBuildInterface.GenerateAssetBundleBuilds()` method to get the set of bundles and assets to build, then modifies `addressableNames` field to set the loading path of the filename instead of the full path.

```csharp
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline;

public static class BuildAssetBundlesExample
{
    public static bool BuildAssetBundles(string outputPath, bool forceRebuild, bool useChunkBasedCompression, BuildTarget buildTarget)
    {
        var options = BuildAssetBundleOptions.None;
        if (useChunkBasedCompression)
            options |= BuildAssetBundleOptions.ChunkBasedCompression;

        if (forceRebuild)
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

        // Get the set of bundle to build
        var bundles = ContentBuildInterface.GenerateAssetBundleBuilds();
        // Update the addressableNames to load by the file name without extension
        for (var i = 0; i < bundles.Length; i++)
            bundles[i].addressableNames = bundles[i].assetNames.Select(Path.GetFileNameWithoutExtension).ToArray();

        var manifest = CompatibilityBuildPipeline.BuildAssetBundles(m_Settings.outputPath, bundles, options, m_Settings.buildTarget);
        return manifest != null;
    }
}
```
