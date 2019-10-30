# Getting started with Scriptable Build Pipeline

## Installing the Scriptable Build Pipeline (SBP) package

Requires Unity 2018.3 or later.

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest).

To build an AssetBundle, use the ContentPipeline.BuildAssetBundles() method. In its simplest form, you supply the following parameters:

* Build Parameters - An object that implements the `IBuildParameters` interface. The object specifies the BuildTarget, the BuildTargetGroup, the output path, and additional optional properties.

* The content to build - An object that implements the `IBundleBuildContent` interface. The object specifies the content to build (the assets) and its layout (what assets in which bundles.)

* A results object - An object that implements the `IBundleBuildResults` interface. The object receives the details of the built AssetBundles.

**Note:** The `UnityEditor.Build.Pipeline` namespace contains default implementations for all of the SBP required interfaces.  Implementation names mirror the interfaces, with the leading 'I' removed. For example, the `IBuildParameters` interface is implemented as `BuildParameters`.

To quickly switch to building AssetBundles with SBP, use the `CompatibilityBuildPipeline.BuildAssetBundles()` method as a drop in replacement for existing code. This method has the nearly identical parameters as the `BuildPipeline.BuildAssetBundles()` method. For additional information, see the [Usage Examples](UsageExamples.md) and [Upgrade Guide](UpgradeGuide.md).