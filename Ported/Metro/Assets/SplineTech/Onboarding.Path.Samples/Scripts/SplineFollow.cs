using Onboarding.BezierPath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineFollow : MonoBehaviour
{
    public PathData pathData;
    public Vector3 worldSpaceOffset;
    public float speed;
    public float targetDistance;

    private float distanceFromSplineOrigin;
    private PathController.LookupCache cameraNavCache;
    private PathController.LookupCache cameraTargetNavCache;

    public void Start()
    {
        distanceFromSplineOrigin = Random.Range(0, pathData.PathLength);
        cameraNavCache = new PathController.LookupCache();
        cameraTargetNavCache = new PathController.LookupCache();
        UpdatePosition();
    }

    public void Update()
    {
        distanceFromSplineOrigin += speed * Time.deltaTime;
        
        if (distanceFromSplineOrigin > pathData.PathLength)
        {
            distanceFromSplineOrigin = distanceFromSplineOrigin % pathData.PathLength;
            PathController.ResetCache(cameraNavCache);
        }
        else if (distanceFromSplineOrigin < 0)
        {
            distanceFromSplineOrigin = pathData.PathLength + (distanceFromSplineOrigin % pathData.PathLength);
            PathController.ResetCache(cameraNavCache);
        }
        
        UpdatePosition();
    }
    private void UpdatePosition()
    {
        PathController.InterpolatePosition(pathData, cameraNavCache, distanceFromSplineOrigin, out var pos);

        float targetDistanceFromSplineOrigin = ((distanceFromSplineOrigin + targetDistance) % pathData.PathLength);

        PathController.InterpolatePosition(pathData, cameraTargetNavCache, targetDistanceFromSplineOrigin, out var targetPos);

        transform.position = pos + worldSpaceOffset;
        transform.LookAt(targetPos);
    }
}
