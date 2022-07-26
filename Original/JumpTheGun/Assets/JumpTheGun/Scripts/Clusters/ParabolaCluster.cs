using Unity.Mathematics;
public class ParabolaCluster 
{
    public static void Create(float startY, float height, float endY, out float a, out float b, out float c) {
        c = startY;
        float k = math.sqrt(math.abs(startY - height)) / (math.sqrt(math.abs(startY - height)) + math.sqrt(math.abs(endY - height)));
        a = (height - startY - k * (endY - startY)) / (k * k - k);
        b = endY - startY - a;
    }
    /// <summary>
    /// Solves a parabola (in the form y = a*t*t + b*t + c) for y.
    /// </summary>
    public static float Solve(float a, float b, float c, float t){
        return a * t * t + b * t + c;
    }

}
