using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineMesh))]
public class SplineMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Unique Mesh"))
        {
            (target as SplineMesh).GenerateMesh();
        }
        if (GUILayout.Button("Generate one mesh per curve"))
        {
            (target as SplineMesh).GenerateSplitMeshesPerCurve();
        }
        if (GUILayout.Button("Display lengths"))
        {
            (target as SplineMesh).DisplayCurvesLength();
        }
        if (GUILayout.Button("Debug Look Up 100m"))
        {
            (target as SplineMesh).DebugLookup(100);
        }

    }
}
