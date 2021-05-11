using UnityEngine;

public static class ParabolaUtil
{
    /// <summary>
    /// Creates a parabola (in the form y = a*t*t + b*t + c) that matches the following conditions:
    /// - Contains the points (0, startY) and (1, endY)
    /// - Reaches the given height.
    /// </summary>
    public static void Create(float startY, float height, float endY, out float a, out float b, out float c)
    {
        c = startY;

        float k = Mathf.Sqrt(Mathf.Abs(startY - height)) / (Mathf.Sqrt(Mathf.Abs(startY - height)) + Mathf.Sqrt(Mathf.Abs(endY - height)));
        a = (height - startY - k * (endY - startY)) / (k * k - k);

        b = endY - startY - a;
    }

    /// <summary>
    /// Creates a parabola with a collision point at time value q and certain height:
    /// - Contains the points (0, startY) and (1, endY)
    /// - Reaches the given height at time q.
    /// </summary>
    public static void CreateParabolaOverPoint(float startY, float q, float height, float endY, out float a, out float b, out float c)
    {
        c = startY;
        a = height - q * endY + q * c - c / (q * q - q);
        b = endY - startY - a;
    }

    /// <summary>
    /// Solves a parabola (in the form y = a*t*t + b*t + c) for y.
    /// </summary>
    public static float Solve(float a, float b, float c, float t)
    {
        return a * t * t + b * t + c;
    }
}
