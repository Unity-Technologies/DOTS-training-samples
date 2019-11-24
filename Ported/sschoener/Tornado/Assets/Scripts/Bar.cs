using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bar
{
    public Point point1;
    public Point point2;
    public float length;
    public Matrix4x4 matrix;
    public float3 oldDelta;
    public Color color;
    public float thickness;

    public void AssignPoints(Point a, Point b)
    {
        point1 = a;
        point2 = b;
        var delta = point2.pos - point1.pos;
        length = math.length(delta);

        thickness = Random.Range(.25f, .35f);

        var pos = (point1.pos + point2.pos) / 2;
        var rot = quaternion.LookRotation(delta, new float3(0, 1, 0));
        var scale = new float3(thickness, thickness, length);
        matrix = float4x4.TRS(pos, rot, scale);

        var proj = math.dot(new float3(0, 1, 0), delta / length);
        float upDot = math.acos(math.abs(proj)) / math.PI;
        color = Color.white * upDot * Random.Range(.7f, 1f);
    }
}
