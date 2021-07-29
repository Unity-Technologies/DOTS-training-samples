using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneDebug))]
public class SceneDebugExample : Editor
{
    GUIStyle style;
    void OnEnable()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.normal.textColor = Color.black;
        }
    }
    
    void OnSceneGUI()
    {
        Handles.Label(Vector3.zero, "I am a scene label at world pos", style);
    }
}
