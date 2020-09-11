#if UNITY_EDITOR

using Unity.Entities;
using UnityEngine;
using UnityEditor;

public class BoardCreationDebug : MonoBehaviour
{
    void OnEnable() => SceneView.duringSceneGui += OnDuringSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnDuringSceneGUI;
    void OnDuringSceneGUI(SceneView obj) => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BoardCreationSystem>().DrawDebugHandles();
}

#endif