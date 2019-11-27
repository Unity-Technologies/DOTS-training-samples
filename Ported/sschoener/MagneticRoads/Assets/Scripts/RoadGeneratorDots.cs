using System;
using UnityEngine;

public class RoadGeneratorDots : MonoBehaviour
{
    void OnDestroy()
    {
        if (Intersections.Occupied.IsCreated)
            Intersections.Occupied.Dispose();
    }
}
