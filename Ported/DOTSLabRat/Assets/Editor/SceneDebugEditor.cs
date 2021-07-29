using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneDebug))]
public class SceneDebugExample : Editor
{
    void OnSceneGUI()
    {
        Handles.Label(Vector3.zero, "I am a scene label at world pos");
    }
}
