using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Unity.Build.Common
{
    public sealed class SceneList : IBuildSettingsComponent
    {
        readonly int m_CurrentPickerWindow;

        [Property]
        public bool BuildCurrentScene { get; set; }

        [Property, AssetGuid(typeof(SceneAsset))]
        public List<GlobalObjectId> Scenes { get; set; } = new List<GlobalObjectId>();

        public string[] GetScenePathsForBuild()
        {
            if (BuildCurrentScene)
            {
                // Build a list of the root scenes
                var rootScenes = new List<string>();
                for (int i = 0; i != EditorSceneManager.sceneCount; i++)
                {
                    var scene = EditorSceneManager.GetSceneAt(i);
                    if (scene.isSubScene)
                        continue;
                    if (!scene.isLoaded)
                        continue;
                    if (EditorSceneManager.IsPreviewScene(scene))
                        continue;
                    if (string.IsNullOrEmpty(scene.path))
                        continue;

                    rootScenes.Add(scene.path);
                }

                return rootScenes.ToArray();
            }
            else
            {
                return Scenes.Select(id => id.assetGUID.ToString()).Select(AssetDatabase.GUIDToAssetPath).ToArray();
            }
        }
    }
}
