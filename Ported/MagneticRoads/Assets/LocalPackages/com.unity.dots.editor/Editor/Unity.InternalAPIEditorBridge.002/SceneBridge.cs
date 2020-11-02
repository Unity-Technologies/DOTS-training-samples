using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Unity.Editor.Bridge
{
    static class SceneBridge
    {
        public static Scene GetSceneByHandle(int handle) => EditorSceneManager.GetSceneByHandle(handle);
    }
}
