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
    private void UpdatePosition()
    {
        if (cameraNavCache==null)
            cameraNavCache = new PathController.LookupCache();
        PathController.InterpolatePositionAndDirection(pathData, cameraNavCache, distanceFromSplineOrigin, out var pos, out var direction);

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
