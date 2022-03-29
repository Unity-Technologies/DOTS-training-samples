using Unity.Entities;
using Unity.Mathematics;

public struct Parabola : IComponentData
{
    private float a;
    private float b;
    private float c;

    /// <summary>
    /// Creates a parabola (in the form y = a*t*t + b*t + c) that matches the following conditions:
    /// - Contains the points (0, startY) and (1, endY)
    /// - Reaches the given height.
    /// </summary>
    public void Create(float startY, float height, float endY)
    {

        c = startY;

        float k = math.sqrt(math.abs(startY - height)) / (math.sqrt(math.abs(startY - height)) + math.sqrt(math.abs(endY - height)));
        a = (height - startY - k * (endY - startY)) / (k * k - k);

        b = endY - startY - a;
    }

    /// <summary>
    /// Solves a parabola (in the form y = a*t*t + b*t + c) for y.
    /// </summary>
    public float Solve(float t)
    {
        return a * t * t + b * t + c;
    }

}
