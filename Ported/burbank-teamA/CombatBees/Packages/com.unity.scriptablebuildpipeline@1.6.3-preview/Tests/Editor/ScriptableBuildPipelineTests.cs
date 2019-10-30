using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.WriteTypes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnityEditor.Build.Pipeline.Tests

{
    [TestFixture]

    class ScriptableBuildPipelineTests
    {
        const string k_FolderPath = "Test";
        const string k_TmpPath = "tmp";

        const string k_ScenePath = "Assets/testScene.unity";
        const string k_TestAssetsPath = "Assets/TestAssetsOnlyWillBeDeleted";
        const string k_CubePath = k_TestAssetsPath + "/Cube.prefab";

        [OneTimeSetUp]
        public void Setup()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            Directory.CreateDirectory(k_TestAssetsPath);
#if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(GameObject.CreatePrimitive(PrimitiveType.Cube), k_CubePath);
#else
            PrefabUtility.CreatePrefab(k_CubePath, GameObject.CreatePrimitive(PrimitiveType.Cube));
#endif
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            AssetDatabase.DeleteAsset(k_ScenePath);
            AssetDatabase.DeleteAsset(k_CubePath);
            AssetDatabase.DeleteAsset(k_TestAssetsPath);

            if (Directory.Exists(k_FolderPath))
                Directory.Delete(k_FolderPath, true);
            if (Directory.Exists(k_TmpPath))
                Directory.Delete(k_TmpPath, true);
        }

        static ReturnCode RunTask<T>(params IContextObject[] args) where T : IBuildTask
        {
            IBuildContext context = new BuildContext(args);
            IBuildTask instance = Activator.CreateInstance<T>();
            ContextInjector.Inject(context, instance);
            var result = instance.Run();
            ContextInjector.Extract(context, instance);
            return result;
        }

        static IBundleBuildParameters GetBuildParameters()
        {
            if (Directory.Exists(k_FolderPath))
                Directory.Delete(k_FolderPath, true);
            if (Directory.Exists(k_TmpPath))
                Directory.Delete(k_TmpPath, true);

            Directory.CreateDirectory(k_FolderPath);
            Directory.CreateDirectory(k_TmpPath);

            IBundleBuildParameters buildParams = new BundleBuildParameters(EditorUserBuildSettings.activeBuildTarget, BuildTargetGroup.Unknown, k_FolderPath);
            buildParams.TempOutputFolder = k_TmpPath;
            return buildParams;
        }

        static IBundleBuildContent GetBundleContent()
        {
            List<AssetBundleBuild> buildData = new List<AssetBundleBuild>();
            AssetBundleBuild dataPoint1 = new AssetBundleBuild()
            {
                addressableNames = new[] { k_CubePath },
                assetBundleName = "bundle",
                assetBundleVariant = "",
                assetNames = new[] { k_CubePath }
            };
            buildData.Add(dataPoint1);
            IBundleBuildContent buildContent = new BundleBuildContent(buildData);
            return buildContent;
        }

        static IDependencyData GetDependencyData()
        {
            GUID guid;
            GUID.TryParse(AssetDatabase.AssetPathToGUID(k_CubePath), out guid);
            ObjectIdentifier[] oId = ContentBuildInterface.GetPlayerObjectIdentifiersInAsset(guid, EditorUserBuildSettings.activeBuildTarget);
            AssetLoadInfo loadInfo = new AssetLoadInfo()
            {
                asset = guid,
                address = k_CubePath,
                includedObjects = oId.ToList(),
                referencedObjects = oId.ToList()
            };

            IDependencyData dep = new BuildDependencyData();
            dep.AssetInfo.Add(guid, loadInfo);

            return dep;
        }

        [UnityTest]
        public IEnumerator BuildPipeline_AssetBundleBuild_DoesNotResetUnsavedScene()
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            yield return null;
            EditorSceneManager.SaveScene(s, k_ScenePath);
            GameObject.CreatePrimitive(PrimitiveType.Cube);
            EditorSceneManager.MarkSceneDirty(s);

            GameObject objectWeAdded = GameObject.Find("Cube");
            Assert.IsNotNull(objectWeAdded, "No object before entering playmode");
            Assert.AreEqual("testScene", SceneManager.GetActiveScene().name);

            IBundleBuildParameters buildParameters = GetBuildParameters();
            IBundleBuildContent buildContent = GetBundleContent();
            IBundleBuildResults results;

            ReturnCode exitCode = ContentPipeline.BuildAssetBundles(buildParameters, buildContent, out results);
            Assert.AreEqual(ReturnCode.UnsavedChanges, exitCode);

            Assert.AreEqual("testScene", SceneManager.GetActiveScene().name);
            objectWeAdded = GameObject.Find("Cube");
            Assert.IsNotNull(objectWeAdded, "No object after entering playmode");
        }

        [UnityTest]
        public IEnumerator ValidationMethods_HasDirtyScenes()
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            yield return null;

            bool dirty = ValidationMethods.HasDirtyScenes();
            Assert.IsFalse(dirty);

            EditorSceneManager.MarkSceneDirty(s);

            dirty = ValidationMethods.HasDirtyScenes();
            Assert.IsTrue(dirty);
        }

        [Test]
        public void DefaultBuildTasks_WriteSerializedFiles()
        {
            IBuildParameters buildParams = GetBuildParameters();
            IDependencyData dependencyData = new BuildDependencyData();
            IWriteData writeData = new BuildWriteData();
            IBuildResults results = new BuildResults();

            ReturnCode exitCode = RunTask<WriteSerializedFiles>(buildParams, dependencyData, writeData, results);
            Assert.AreEqual(ReturnCode.Success, exitCode);
        }

        [Test]
        public void DefaultBuildTasks_GenerateBundlePacking()
        {
            IBundleBuildContent buildContent = GetBundleContent();
            IDependencyData dep = GetDependencyData();
            IBundleWriteData writeData = new BundleWriteData();
            IDeterministicIdentifiers deterministicId = new PrefabPackedIdentifiers();

            ReturnCode exitCode = RunTask<GenerateBundlePacking>(buildContent, dep, writeData, deterministicId);
            Assert.AreEqual(ReturnCode.Success, exitCode);
        }

        [Test]
        public void DefaultBuildTasks_GenerateBundleCommands()
        {
            IBundleBuildContent buildContent = GetBundleContent();
            IDependencyData dep = GetDependencyData();
            IBundleWriteData writeData = new BundleWriteData();
            IDeterministicIdentifiers deterministicId = new PrefabPackedIdentifiers();

            RunTask<GenerateBundlePacking>(buildContent, dep, writeData, deterministicId);

            ReturnCode exitCode = RunTask<GenerateBundleCommands>(buildContent, dep, writeData, deterministicId);
            Assert.AreEqual(ReturnCode.Success, exitCode);
        }

        [Test]
        public void DefaultBuildTasks_GenerateBundleMaps()
        {
            IDependencyData dep = GetDependencyData();
            IBundleWriteData writeData = new BundleWriteData();

            ReturnCode exitCode = RunTask<GenerateBundleMaps>(dep, writeData);
            Assert.AreEqual(ReturnCode.Success, exitCode);
        }

        [Test]
        public void DefaultBuildTasks_PostPackingCallback()
        {
            bool packingCallbackCalled = false;

            IBuildParameters buildParams = GetBuildParameters();
            IDependencyData dep = GetDependencyData();
            IBundleWriteData writeData = new BundleWriteData();
            BuildCallbacks callback = new BuildCallbacks();
            callback.PostPackingCallback = (parameters, data, arg3) =>
            {
                packingCallbackCalled = true;
                return ReturnCode.Success;
            };

            ReturnCode exitCode = RunTask<PostPackingCallback>(buildParams, dep, writeData, callback);
            Assert.AreEqual(ReturnCode.Success, exitCode);
            Assert.IsTrue(packingCallbackCalled);
        }

        [Test]
        public void DefaultBuildTasks_PostWritingCallback()
        {
            bool writingCallbackCalled = false;

            IBuildParameters buildParams = GetBuildParameters();
            IDependencyData dep = GetDependencyData();
            IWriteData writeData = new BuildWriteData();
            IBuildResults results = new BuildResults();
            BuildCallbacks callback = new BuildCallbacks();
            callback.PostWritingCallback = (parameters, data, arg3, arg4) =>
            {
                writingCallbackCalled = true;
                return ReturnCode.Success;
            };

            ReturnCode exitCode = RunTask<PostWritingCallback>(buildParams, dep, writeData, results, callback);
            Assert.AreEqual(ReturnCode.Success, exitCode);
            Assert.IsTrue(writingCallbackCalled);
        }

        [Test]
        public void DefaultBuildTasks_PostDependencyCallback()
        {
            bool dependencyCallbackCalled = false;

            IBuildParameters buildParameters = GetBuildParameters();
            IDependencyData dep = GetDependencyData();
            BuildCallbacks callback = new BuildCallbacks();
            callback.PostDependencyCallback = (parameters, data) =>
            {
                dependencyCallbackCalled = true;
                return ReturnCode.Success;
            };

            ReturnCode exitCode = RunTask<PostDependencyCallback>(buildParameters, dep, callback);
            Assert.AreEqual(ReturnCode.Success, exitCode);
            Assert.IsTrue(dependencyCallbackCalled);
        }

        [Test]
        public void DefaultBuildTasks_PostScriptsCallbacks()
        {
            bool scriptsCallbackCalled = false;

            IBuildParameters buildParameters = GetBuildParameters();
            IBuildResults results = new BuildResults();
            BuildCallbacks callback = new BuildCallbacks();
            callback.PostScriptsCallbacks = (parameters, buildResults) =>
            {
                scriptsCallbackCalled = true;
                return ReturnCode.Success;
            };

            ReturnCode exitCode = RunTask<PostScriptsCallback>(buildParameters, results, callback);
            Assert.AreEqual(ReturnCode.Success, exitCode);
            Assert.IsTrue(scriptsCallbackCalled);
        }

        [Test]
        public void DefaultBuildTasks_AppendBundleHash()
        {
            IBundleBuildParameters buildParameters = GetBuildParameters();
            buildParameters.AppendHash = true;
            var fileName = k_FolderPath + "/TestBundle";
            var fileHash = HashingMethods.Calculate(fileName).ToHash128();
            File.WriteAllText(fileName, fileName);
            IBundleBuildResults results = new BundleBuildResults();
            results.BundleInfos["TestBundle"] = new BundleDetails
            {
                Crc = 0,
                FileName = fileName,
                Hash = fileHash
            };

            ReturnCode exitCode = RunTask<AppendBundleHash>(buildParameters, results);
            Assert.AreEqual(ReturnCode.Success, exitCode);
            FileAssert.Exists(fileName + "_" + fileHash);
        }

        [Test]
        public void HashChanges_WhenDependencyListHasModifiedEntries()
        {
            Dictionary<string, ulong> offsets = new Dictionary<string, ulong>();
            ResourceFile[] resourceFiles = new ResourceFile[0];

            string[] dependencies = new string[]
            {
                "dependency1",
                "dependency2"
            };

            Hash128 firstHash = ArchiveAndCompressBundles.CalculateHashVersion(offsets, resourceFiles, dependencies);

            dependencies[1] = "newDependency";

            Hash128 secondHash = ArchiveAndCompressBundles.CalculateHashVersion(offsets, resourceFiles, dependencies);

            Assert.AreNotEqual(firstHash, secondHash);
        }

        [Test]
        public void HashRemainsTheSame_AfterRevertingDependencyListChange()
        {
            Dictionary<string, ulong> offsets = new Dictionary<string, ulong>();
            ResourceFile[] resourceFiles = new ResourceFile[0];

            string[] dependencies = new string[]
            {
                "dependency1",
                "dependency2"
            };

            Hash128 firstHash = ArchiveAndCompressBundles.CalculateHashVersion(offsets, resourceFiles, dependencies);

            dependencies[1] = "newDependency";

            Hash128 secondHash = ArchiveAndCompressBundles.CalculateHashVersion(offsets, resourceFiles, dependencies);

            dependencies[1] = "dependency2";

            Hash128 thirdHash = ArchiveAndCompressBundles.CalculateHashVersion(offsets, resourceFiles, dependencies);

            Assert.AreNotEqual(secondHash, thirdHash);
            Assert.AreEqual(firstHash, thirdHash);
        }

        [UnityTest]
        public IEnumerator SceneDataWriteOperation_HashChanges_WhenPrefabDepenencyChanges()
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            yield return null;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_CubePath);
            prefab.transform.position = new Vector3(0, 0, 0);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            PrefabUtility.InstantiatePrefab(prefab);

            EditorSceneManager.SaveScene(s, k_ScenePath);

            var op = new SceneDataWriteOperation
            {
                Command = new WriteCommand(),
                PreloadInfo = new PreloadInfo(),
#if !UNITY_2019_3_OR_NEWER
                ProcessedScene = k_ScenePath,
#endif
                ReferenceMap = new BuildReferenceMap(),
                UsageSet = new BuildUsageTagSet(),
                Scene = k_ScenePath
            };
            var cacheVersion1 = op.GetHash128();

            prefab.transform.position = new Vector3(1, 1, 1);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            var cacheVersion2 = op.GetHash128();

            Assert.AreNotEqual(cacheVersion1, cacheVersion2);
        }

        [UnityTest]
        public IEnumerator SceneBundleWriteOperation_HashChanges_WhenPrefabDepenencyChanges()
        {
            Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            yield return null;

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_CubePath);
            prefab.transform.position = new Vector3(0, 0, 0);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            PrefabUtility.InstantiatePrefab(prefab);

            EditorSceneManager.SaveScene(s, k_ScenePath);

            var op = new SceneBundleWriteOperation
            {
                Command = new WriteCommand(),
                PreloadInfo = new PreloadInfo(),
#if !UNITY_2019_3_OR_NEWER
                ProcessedScene = k_ScenePath,
#endif
                ReferenceMap = new BuildReferenceMap(),
                UsageSet = new BuildUsageTagSet(),
                Info = new SceneBundleInfo(),
                Scene = k_ScenePath
            };
            var cacheVersion1 = op.GetHash128();

            prefab.transform.position = new Vector3(1, 1, 1);
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            var cacheVersion2 = op.GetHash128();

            Assert.AreNotEqual(cacheVersion1, cacheVersion2);
        }
    }
}
