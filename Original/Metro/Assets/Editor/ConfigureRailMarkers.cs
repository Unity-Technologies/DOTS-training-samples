using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ConfigureRailMarkers : MonoBehaviour {
	
	[MenuItem("GameObject/SetupRailMarkers",false,10)]
	public static void DoConfig()
	{
		GameObject[] SortedGameObjects = Selection.gameObjects.OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
		int metroLine = SortedGameObjects[0].GetComponent<RailMarker>().metroLineID;
		for (int i = 0; i < SortedGameObjects.Length; i++)
		{
			SortedGameObjects[i].name = metroLine + "_" + i;
			SortedGameObjects[i].GetComponent<RailMarker>().pointIndex = i;
		}
	}
}
