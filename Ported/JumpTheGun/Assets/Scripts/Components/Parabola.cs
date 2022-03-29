using Unity.Entities;
using Unity.Mathematics;

public struct ParabolaData : IComponentData
{
    public float a;
    public float b;
    public float c;
}

public struct Parabola
{
    /// <summary>
    /// Creates a parabola (in the form y = a*t*t + b*t + c) that matches the following conditions:
    /// - Contains the points (0, startY) and (1, endY)
    /// - Reaches the given height.
    /// </summary>
    public static ParabolaData Create(float startY, float height, float endY)
    {
        ParabolaData data = new ParabolaData();

        data.c = startY;

        float k = math.sqrt(math.abs(startY - height)) / (math.sqrt(math.abs(startY - height)) + math.sqrt(math.abs(endY - height)));
        data.a = (height - startY - k * (endY - startY)) / (k * k - k);

        data.b = endY - startY - data.a;

        return data;
    }

    /// <summary>
    /// Solves a parabola (in the form y = a*t*t + b*t + c) for y.
    /// </summary>
    public static float Solve(in ParabolaData data, float t)
    {
        return data.a * t * t + data.b * t + data.c;
    }
}