using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.SceneManagement;

[ScriptedImporter(39, "sceneMetaData")]
class SceneMetaDataImporter : ScriptedImporter
{
    public static readonly int CurrentFileFormatVersion = 1;

    [Serializable]
    public struct SceneMetaData
    {
        public BlobArray<Hash128> SubScenes;
    }

    public unsafe static Hash128[] GetSubSceneGuids(string guid)
    {
        var hash = AssetDatabaseExperimental.GetArtifactHash(guid, typeof(SceneMetaDataImporter), AssetDatabaseExperimental.ImportSyncMode.Block);
        AssetDatabaseExperimental.GetArtifactPaths(hash, out string[] paths);

        var metaPath = paths.First(o => o.EndsWith("scenemeta"));

        BlobAssetReference<SceneMetaData> sceneMetaDataRef;
        if (!BlobAssetReference<SceneMetaData>.TryRead(metaPath, SceneMetaDataImporter.CurrentFileFormatVersion, out sceneMetaDataRef))
            return new Hash128[0];

        Hash128[] guids = sceneMetaDataRef.Value.SubScenes.ToArray();
        sceneMetaDataRef.Dispose();
        return guids;
    }

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var scene = EditorSceneManager.OpenScene(ctx.assetPath, OpenSceneMode.Additive);
        try
        {
            var dependencies = AssetDatabase.GetDependencies(scene.path).Where(x => x.ToLower().EndsWith(".prefab"));
            foreach (var dependency in dependencies)
                ctx.DependsOnSourceAsset(dependency);

            var metaPath = ctx.GetResultPath("scenemeta");
            var subScenes = SubScene.AllSubScenes;
            var sceneGuids = subScenes.Select(x => x.SceneGUID).Distinct().ToArray();

            var builder = new BlobBuilder(Allocator.Temp);
            ref var metaData = ref builder.ConstructRoot<SceneMetaData>();

            builder.Construct(ref metaData.SubScenes, sceneGuids);
            BlobAssetReference<SceneMetaData>.Write(builder, metaPath, SceneMetaDataImporter.CurrentFileFormatVersion);
            builder.Dispose();
        }
        finally
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}