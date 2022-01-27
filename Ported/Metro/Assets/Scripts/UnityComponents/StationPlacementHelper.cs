using Onboarding.BezierPath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StationPlacementHelper : MonoBehaviour
{
    public PathData pathData;
    public float distanceFromSplineOrigin;
    private PathController.LookupCache cameraNavCache;

    public void Start()
    {
        cameraNavCache = new PathController.LookupCache();
        UpdatePosition();
    }

    public void Update()
    {
        distanceFromSplineOrigin = Mathf.Clamp(distanceFromSplineOrigin, 0, pathData.PathLength);
       
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        if (cameraNavCache==null)
            cameraNavCache = new PathController.LookupCache();
        PathController.InterpolatePositionAndDirection(pathData, cameraNavCache, distanceFromSplineOrigin, out var pos, out var direction);

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(StationPlacementHelper))]
[UnityEditor.CanEditMultipleObjects]
public class StationPlacementHelperEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (targets.Length>1 && GUILayout.Button("Align Selected together"))
        {
            // Fetch the position of the first entry
            StationPlacementHelper firstStation = targets[0] as StationPlacementHelper;
            var currentWorldPosition = firstStation.transform.position;

            for (int i = 1; i < targets.Length; ++i)
            {
                StationPlacementHelper currStation = targets[i] as StationPlacementHelper;
                
                currStation.distanceFromSplineOrigin = PathController.FindSplineDistanceFromPoint(currStation.pathData, currentWorldPosition, currStation.distanceFromSplineOrigin);
                currStation.UpdatePosition();
                UnityEditor.EditorUtility.SetDirty(currStation);
                UnityEditor.EditorUtility.SetDirty(currStation.gameObject);
                currentWorldPosition = currStation.transform.position;
            }
        }
    }
}
#endif
