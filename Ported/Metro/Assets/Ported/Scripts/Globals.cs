using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public static float BEZIER_HANDLE_REACH = 0.1f;
    public static float BEZIER_PLATFORM_OFFSET = 3f;
    public static float PLATFORM_ADJACENCY_LIMIT = 12f;
    public const int BEZIER_MEASUREMENT_SUBDIVISIONS = 2;
    public const float PLATFORM_ARRIVAL_THRESHOLD = 0.975f;
    public const float RAIL_SPACING = 0.5f;

    public static BezierPath[] Paths;
}