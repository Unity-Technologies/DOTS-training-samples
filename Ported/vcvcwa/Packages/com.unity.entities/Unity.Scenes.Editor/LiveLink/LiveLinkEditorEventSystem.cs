using UnityEditor;

namespace Unity.Scenes.Editor
{
    static class LiveLinkEditorEventSystem
    {
        [InitializeOnLoadMethod]
        static void InitializeLiveLinkSystems()
        {
            var sendSystem = EditorSceneLiveLinkToPlayerSendSystem.instance;
            var buildSystem = LiveLinkAssetBundleBuildSystem.instance;
        }
    }
}