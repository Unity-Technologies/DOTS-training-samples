using System;
using System.IO;
using NUnit.Framework;
using Unity.Build;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.Editor.Tests.LiveLink
{
    abstract class LiveLinkTestFixture
    {
        string m_ValueBeforeTests;

        protected DirectoryInfo TestDirectory;

        [SetUp]
        public void Setup()
        {
            m_ValueBeforeTests = LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid;
            TestDirectory = new DirectoryInfo("Assets/TestBuildSettings");
            if (!TestDirectory.Exists)
                TestDirectory.Create();
        }

        [TearDown]
        public void Teardown()
        {
            LiveLinkSettings.Instance.SelectedBuildSettingsAssetGuid = m_ValueBeforeTests;
            AssetDatabase.DeleteAsset(TestDirectory.GetRelativePath());
        }

        protected (BuildSettings asset, string guid) CreateBuildSettings(Action<BuildSettings> mutator = null)
        {
            var assetPath = Path.Combine(TestDirectory.GetRelativePath(), Path.ChangeExtension(Path.GetRandomFileName(), BuildSettingsScriptedImporter.BuildSettingsExtension));
            var buildSettings = ScriptableObject.CreateInstance<BuildSettings>();
            mutator?.Invoke(buildSettings);
            buildSettings.SerializeToPath(assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var loadedBuildSetting = AssetDatabase.LoadAssetAtPath<BuildSettings>(assetPath);

            return (loadedBuildSetting, AssetDatabase.AssetPathToGUID(assetPath));
        }
    }
}