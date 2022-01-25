using Onboarding.BezierPath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAtArcLength : MonoBehaviour
{
    public enum Method
    { 
        Parametric,
        ArcLength
    };

    public Method SpawnMethod;
    public Material material;
    public float objectSize;
    [Range(0, 1)] public float startDistance;
    [Range(0, 1)] public float endDistance;
    public int count;
    public PathData pathData;

    public void Start()
    {
        float distanceIncrement = count == 1 ? 0 : (endDistance - startDistance) / (count-1);
        float dist = startDistance;

        for (int i = 0; i < count; i++, dist += distanceIncrement)
        {
            Vector3 position;
            if (SpawnMethod == Method.ArcLength)
            {
                int dummy = 0;
                PathController.InterpolatePosition(pathData, ref dummy, dist * pathData.PathLength, out position);
            }
            else
                PathController.InterpolatePosition(pathData, 0, dist, out position);
            var spawnObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spawnObject.name = $"Spawnee_{i}";
            spawnObject.transform.localScale = Vector3.one * objectSize;
            spawnObject.transform.position = position;

            var renderer = spawnObject.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
        }
    }

}
