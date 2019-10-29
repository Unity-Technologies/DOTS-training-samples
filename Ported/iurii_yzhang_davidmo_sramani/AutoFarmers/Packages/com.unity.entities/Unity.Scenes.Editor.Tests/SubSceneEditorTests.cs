using System.IO;
using NUnit.Framework;
using Unity.Scenes.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SubSceneEditorTests
{
    string m_TempAssetDir;

    [OneTimeSetUp]
    public void SetUp()
    {
        var guid = AssetDatabase.CreateFolder("Assets", Path.GetRandomFileName());
        m_TempAssetDir = AssetDatabase.GUIDToAssetPath(guid);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AssetDatabase.DeleteAsset(m_TempAssetDir);
    }

    void CreateSubScene(string name)
    {
        var mainScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        EditorSceneManager.SetActiveScene(mainScene);

        var path = Path.Combine(m_TempAssetDir, $"{name}.unity");
        EditorSceneManager.SaveScene(mainScene, path);

        Selection.activeObject = new GameObject();

        SubSceneContextMenu.CreateSubSceneAndAddSelection(Selection.activeObject);
    }

    [Test]
    public void MissingSubSceneFolder()
    {
        Assert.DoesNotThrow(() => CreateSubScene("whatever"));
    }

    [Test]
    public void ExistingSubSceneFolder()
    {
        Directory.CreateDirectory(Path.Combine(m_TempAssetDir, "MatchingCapitalization"));
        Assert.DoesNotThrow(() => CreateSubScene("MatchingCapitalization"));
    }

    [Test]
    public void WrongCapitalizationSubSceneFolder()
    {
        Directory.CreateDirectory(Path.Combine(m_TempAssetDir, "LOWERCASE"));
        Assert.DoesNotThrow(() =>  CreateSubScene("lowercase"));
    }
}
