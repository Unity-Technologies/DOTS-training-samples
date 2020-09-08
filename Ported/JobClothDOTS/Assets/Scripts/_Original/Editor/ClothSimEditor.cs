using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ClothSim))]
public class ClothSimEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.HelpBox("This is an example of a naive implementation of a cloth sim using the Jobs system!  Its biggest limitation is explained below.",MessageType.Error);
		EditorGUILayout.HelpBox("\nOur scene has two copies of the text.  One version (red) uses a single combined mesh for everything, and the second version (blue) uses a separate mesh for each island.  The naive implementation of the cloth solver uses a single IJob for solving all of a mesh's distance constraints, which means that a large amount of the work must run on a single background thread.\n\nBecause of this, the text which has been split into multiple meshes performs much faster: Unity can send multiple Jobs to multiple threads, utilizing more of your available CPU power.\n\n(Check the Profiler in CPU-Timeline mode to see how Unity is organizing its Worker Threads - particularly, compare how it looks when you only have one of the two versions of the text enabled.)\n",MessageType.Info);
		EditorGUILayout.HelpBox("\nAn implementation which made the distance-constraint routine into an IJobParallelFor could avoid this problem - the second version of this demo will show how to achieve that.\n",MessageType.Info);
	}
}
